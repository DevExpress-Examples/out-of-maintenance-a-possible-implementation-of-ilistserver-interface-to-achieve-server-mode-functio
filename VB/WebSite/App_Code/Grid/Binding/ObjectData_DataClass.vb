Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
	Public Class MyDataClass
		Public Function GetMyData(ByVal StartRow As Integer, ByVal PageSize As Integer) As DataView
			Return GetData(StartRow, PageSize)
		End Function
		Private Function GetData(ByVal StartRow As Integer, ByVal PageSize As Integer) As DataView
			Dim Contacts As DataSet = New DataSet()
			Contacts.ReadXml(HttpContext.Current.Server.MapPath("~/App_Data/ContactsList.xml"))

			Dim ContactsTable As DataTable = Contacts.Tables(0)
			Dim PagedContactsTable As DataTable = ContactsTable.Clone()

			Dim i As Integer = StartRow
			Do While i < StartRow + PageSize AndAlso i < ContactsTable.Rows.Count
				PagedContactsTable.ImportRow(ContactsTable.Rows(i))
				i += 1
			Loop

			Return PagedContactsTable.DefaultView
		End Function
		Public Function GetRowsCount() As Integer
			Dim Contacts As DataSet = New DataSet()
			Contacts.ReadXml(HttpContext.Current.Server.MapPath("~/App_Data/ContactsList.xml"))
			Return Contacts.Tables(0).Rows.Count
		End Function
	End Class


