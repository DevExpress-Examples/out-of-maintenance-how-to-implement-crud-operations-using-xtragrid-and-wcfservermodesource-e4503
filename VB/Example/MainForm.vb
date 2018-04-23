' Developer Express Code Central Example:
' How to implement CRUD operations using XtraGrid and WCF Data Services
' 
' This example is similar to the http://www.devexpress.com/scid=E3866 example and
' demonstrates how to implement CRUD operations using XtraGrid and WCF Data
' Services.
' 
' This example works with the standard SQL Northwind database.
' 
' You can find sample updates and versions for different programming languages here:
' http://www.devexpress.com/example=E4365


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.Xpf.Core.ServerMode
Imports DevExpress.Data.WcfLinq
Imports System.Data.Services.Client
Imports Example.ServiceReference1

Namespace Example
	Partial Public Class MainForm
		Inherits Form
		Private context As NorthwindEntities
		Private wSMS As WcfServerModeSource
		Private newCustomer As Customers
		Private customerToEdit As Customers
		Private OldRowCount As Integer = 0
		Private newCustomerKey As String = String.Empty
		Private SelectedRow As Integer = -1
		Public Sub New()
			InitializeComponent()
			context = New NorthwindEntities(New Uri("http://localhost:56848/NorthwindWcfDataService.svc/"))
			wSMS = New WcfServerModeSource()
			wSMS.KeyExpression &= "CustomerID"
			wSMS.Query = context.Customers
			gridControl1.DataSource = wSMS
		End Sub
		Private Sub simpleButton1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles simpleButton1.Click
			newCustomer = CreateNewCustomer()
			OldRowCount = gridView1.DataRowCount
			newCustomerKey = newCustomer.CustomerID
			EditCustomer(newCustomer, "Add new customer", AddressOf CloseAddNewCustomerHandler)
		End Sub
		Private Sub CloseAddNewCustomerHandler(ByVal sender As Object, ByVal e As EventArgs)
			If (CType(sender, EditForm)).DialogResult = System.Windows.Forms.DialogResult.OK Then
				context.AddToCustomers(newCustomer)
				SaveChandes()
			End If
			newCustomer = Nothing
		End Sub
		Private Function CreateNewCustomer() As Customers
			Dim newCustomer = New Customers()
			newCustomer.CustomerID = GenerateCustomerID()
			Return newCustomer
		End Function

        Private Function GenerateCustomerID() As String
            Const IDLength As Integer = 5
            Dim result = String.Empty
            Dim rnd = New Random()
            Dim i = 0
            For i = 0 To IDLength - 1
                result += Convert.ToChar(rnd.Next(65, 90))
            Next i
            Return result
        End Function
        Private Sub EditCustomer(ByVal customer As Customers, ByVal windowTitle As String, ByVal closedDelegate As FormClosingEventHandler)
            Dim frm = New EditForm(customer)
            frm.Text = windowTitle
            AddHandler CType(frm, EditForm).FormClosing, closedDelegate
            frm.ShowDialog()
        End Sub

        Private Sub SaveChandes()
            Dim asyncResult As IAsyncResult = Nothing
            Try
                asyncResult = context.BeginSaveChanges(AddressOf SaveCallback, Nothing)
            Catch ex As Exception
                context.CancelRequest(asyncResult)
                HandleException(ex)
            End Try
        End Sub

        Private Sub SaveCallback(ByVal asyncResult As IAsyncResult)
            Dim response As DataServiceResponse = Nothing
            Try
                response = context.EndSaveChanges(asyncResult)
            Catch ex As Exception
                gridControl1.BeginInvoke(CType(Sub() context.CancelRequest(asyncResult), MethodInvoker))
                HandleException(ex)
                gridControl1.BeginInvoke(CType(Sub() DetachFailedEntities(), MethodInvoker))
            End Try
            gridControl1.BeginInvoke(CType(Sub() wSMS.Reload(), MethodInvoker))
            gridControl1.BeginInvoke(CType(Function() AnonymousMethod3(), MethodInvoker))
        End Sub
        Private Function AnonymousMethod3() As Boolean
            If SelectedRow > 0 Then
                gridView1.FocusedRowHandle = SelectedRow
            End If
            If (Not String.IsNullOrEmpty(newCustomerKey)) AndAlso gridView1.DataRowCount > OldRowCount Then
                For i As Integer = 0 To gridView1.DataRowCount - 1
                    If newCustomerKey = gridView1.GetRowCellValue(i, gridView1.Columns("CustomerID")).ToString() Then
                        gridView1.FocusedRowHandle = i
                        OldRowCount = gridView1.DataRowCount
                        newCustomerKey = String.Empty
                        Exit For
                    End If
                Next i
            End If
            Return True
        End Function


        Private Sub DetachFailedEntities()
            For Each entityDescriptor As EntityDescriptor In context.Entities
                If entityDescriptor.State <> EntityStates.Unchanged Then
                    context.Detach(entityDescriptor.Entity)
                End If
            Next entityDescriptor
        End Sub

        Private Sub HandleException(ByVal ex As Exception)
            gridControl1.BeginInvoke(CType(Sub() MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK), MethodInvoker))
        End Sub
        Private Sub simpleButton2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles simpleButton2.Click
            SelectedRow = gridView1.FocusedRowHandle
            EditSelectedCustomer(gridView1.FocusedRowHandle)
        End Sub

        Private Sub EditSelectedCustomer(ByVal rowHandle As Integer)
            If rowHandle < 0 Then
                Return
            End If
            FindCustomerByIDAndProcess(GetCustomerIDByRowHandle(rowHandle), Sub(customer)
                                                                                customerToEdit = customer
                                                                                EditCustomer(customerToEdit, "Edit customer", AddressOf CloseEditCustomerHandler)
                                                                            End Sub)
        End Sub

        Private Function GetCustomerIDByRowHandle(ByVal rowHandle As Integer) As String
            Return CStr(gridView1.GetRowCellValue(rowHandle, "CustomerID"))
        End Function

        Private Sub CloseEditCustomerHandler(ByVal sender As Object, ByVal e As EventArgs)
            If (CType(sender, EditForm)).DialogResult = System.Windows.Forms.DialogResult.OK Then
                context.UpdateObject(customerToEdit)
                SaveChandes()

            End If
            customerToEdit = Nothing
        End Sub

        Private Sub FindCustomerByIDAndProcess(ByVal customerID As String, ByVal action As Action(Of Customers))
            Dim query As DataServiceQuery(Of Customers) = CType(context.Customers.Where(Function(customer) customer.CustomerID = customerID), DataServiceQuery(Of Customers))
            Try
                query.BeginExecute(AddressOf FindCustomerByIDCallback, New QueryAction(query, action))
            Catch ex As Exception
                HandleException(ex)
            End Try
        End Sub

        Private Sub FindCustomerByIDCallback(ByVal ar As IAsyncResult)
            Dim state = CType(ar.AsyncState, QueryAction)
            Dim customers = state.Query.EndExecute(ar)
            For Each customer As Customers In customers
                Try
                    gridControl1.BeginInvoke(CType(Function() AnonymousMethod6(state, customer), MethodInvoker))
                Catch ex As Exception
                    HandleException(ex)
                End Try
            Next customer
        End Sub

        Private Function AnonymousMethod6(ByVal state As QueryAction, ByVal customer As Customers) As Boolean
            state.Action(customer)
            Return True
        End Function


        Private Sub simpleButton3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles simpleButton3.Click
            SelectedRow = gridView1.FocusedRowHandle
            DeleteSelectedCustomer(gridView1.FocusedRowHandle)

        End Sub

        Private Sub DeleteSelectedCustomer(ByVal rowHandle As Integer)
            If rowHandle < 0 Then
                Return
            End If
            If MessageBox.Show("Do you really want to delete the selected customer?", "Delete Customer", MessageBoxButtons.OKCancel) <> System.Windows.Forms.DialogResult.OK Then
                Return
            End If
            FindCustomerByIDAndProcess(GetCustomerIDByRowHandle(rowHandle), Sub(customer)
                                                                                context.DeleteObject(customer)
                                                                                SaveChandes()
                                                                            End Sub)
        End Sub
    End Class
    Public Class QueryAction
        Private query_Renamed As DataServiceQuery(Of Customers)

        Private action_Renamed As Action(Of Customers)

        Public Sub New(ByVal query As DataServiceQuery(Of Customers), ByVal action As Action(Of Customers))
            Me.query_Renamed = query
            Me.action_Renamed = action
        End Sub

        Public ReadOnly Property Query() As DataServiceQuery(Of Customers)
            Get
                Return query_Renamed
            End Get
        End Property

        Public Sub Action(customer As Customers)
            action_Renamed(customer)
        End Sub
    End Class
End Namespace

