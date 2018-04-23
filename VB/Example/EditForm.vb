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
Imports Example.ServiceReference1

Namespace Example
	Partial Public Class EditForm
		Inherits Form
		Public Sub New(ByVal customer As Customers)
			InitializeComponent()
			teComN.DataBindings.Add("EditValue", customer, "CompanyName")
			teConN.DataBindings.Add("EditValue", customer, "ContactName")
			teAdd.DataBindings.Add("EditValue", customer, "Address")
			teC.DataBindings.Add("EditValue", customer, "Country")
		End Sub
	End Class
End Namespace
