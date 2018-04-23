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
Imports System.Linq
Imports System.Windows.Forms

Namespace Example
	Friend NotInheritable Class Program
		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		Private Sub New()
		End Sub
		<STAThread> _
		Shared Sub Main()
			Application.EnableVisualStyles()
			Application.SetCompatibleTextRenderingDefault(False)
			Application.Run(New MainForm())
		End Sub
	End Class
End Namespace
