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
Imports System.Collections
Imports System.Security.Permissions
Imports System.ComponentModel
Imports DevExpress.Web.ASPxClasses.Internal
Imports DevExpress.Data
Imports System.Collections.Generic

Namespace DevExpress.Web
	<PersistChildren(False), ParseChildren(True), DisplayName("ObjectDataSource wrapper"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level := AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level := AspNetHostingPermissionLevel.Minimal)> _
	Public Class ObjectDataSourceWrapper
		Inherits DataSourceControl
		Private dataSourceId_Renamed As String = String.Empty
		Private dataSource_Renamed As ObjectDataSource = Nothing
		Private pageSize_Renamed As Integer = 10
		Public Sub New()
		End Sub
		Protected Overrides Function GetView(ByVal viewName As String) As DataSourceView
			If (Not viewName Is Nothing) AndAlso ((viewName.Length = 0) OrElse String.Equals(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase)) Then
				Return Me.GetView()
			End If
			Throw New ArgumentException("", "viewName")
		End Function

		Public Property DataSource() As ObjectDataSource
			Get
				Return dataSource_Renamed
			End Get
			Set(ByVal value As ObjectDataSource)
				dataSource_Renamed = value
			End Set
		End Property
		<DefaultValue(10)> _
		Public Property PageSize() As Integer
			Get
				Return pageSize_Renamed
			End Get
			Set(ByVal value As Integer)
				If value < 1 Then
				value = 1
				End If
				pageSize_Renamed = value
			End Set
		End Property
		<DefaultValue(""), Themeable(False), Category("Data"), AutoFormatDisable, Localizable(False)> _
		Public Overridable Property DataSourceID() As String
			Get
				Return dataSourceId_Renamed
			End Get
			Set(ByVal value As String)
				dataSourceId_Renamed = value
			End Set
		End Property
		Private _view As ObjectDataSourceWrapperView
		Private Overloads Function GetView() As ObjectDataSourceWrapperView
			If Me._view Is Nothing Then
				Me._view = New ObjectDataSourceWrapperView(Me, "DefaultView", Me.Context)
			End If
			Return Me._view
		End Function
		Protected Overrides Function GetViewNames() As ICollection
			Return New String() { "DefaultView" }
		End Function
		Protected Friend Overridable Function GetDataSource() As ObjectDataSource
			If Not DataSource Is Nothing Then
			Return DataSource
			End If
			If (Not String.IsNullOrEmpty(DataSourceID)) Then
			Return TryCast(DataControlHelper.FindControl(Me, DataSourceID), ObjectDataSource)
			End If
			Return Nothing
		End Function
	End Class
	<AspNetHostingPermission(SecurityAction.InheritanceDemand, Level := AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level := AspNetHostingPermissionLevel.Minimal)> _
	Public Class ObjectDataSourceWrapperView
		Inherits DataSourceView
		Implements IListServer, ITypedList
		Private owner As ObjectDataSourceWrapper
		Private context As HttpContext
		Private dataSource_Renamed As ObjectDataSource
		Private dataSourceView_Renamed As ObjectDataSourceView
		Private dataRecords_Renamed As List(Of Object)
		Private startIndex As Integer = 0, totalCount As Integer = -1
		Public Sub New(ByVal owner As ObjectDataSourceWrapper, ByVal name As String, ByVal context As HttpContext)
			MyBase.New(owner, name)
			Me.owner = owner
			Me.context = context
			Me.dataRecords_Renamed = New List(Of Object)()
		End Sub

		Protected ReadOnly Property DataSource() As ObjectDataSource
			Get
				Return dataSource_Renamed
			End Get
		End Property
		Protected ReadOnly Property DataSourceView() As ObjectDataSourceView
			Get
				Return dataSourceView_Renamed
			End Get
		End Property
		Protected Overrides Function ExecuteSelect(ByVal arguments As DataSourceSelectArguments) As IEnumerable
			Me.dataSource_Renamed = owner.GetDataSource()
			If Not Me.dataSource_Renamed Is Nothing Then
				Me.dataSourceView_Renamed = TryCast((CType(DataSource, IDataSource)).GetView("DefaultView"), ObjectDataSourceView)
			End If
			Return Me
		End Function
		Protected Overrides Function ExecuteUpdate(ByVal keys As IDictionary, ByVal values As IDictionary, ByVal oldValues As IDictionary) As Integer
			If DataSourceView Is Nothing Then
				Throw New NotSupportedException()
			End If
			Return DataSourceView.Update(keys, values, oldValues)
		End Function
		Protected Overrides Function ExecuteInsert(ByVal values As IDictionary) As Integer
			If DataSourceView Is Nothing Then
				Throw New NotSupportedException()
			End If
			Return DataSourceView.Insert(values)
		End Function

		Protected Overrides Function ExecuteDelete(ByVal keys As IDictionary, ByVal oldValues As IDictionary) As Integer
			If DataSourceView Is Nothing Then
				Throw New NotSupportedException()
			End If
			Return DataSourceView.Delete(keys, oldValues)
		End Function
		Private Sub CriteriaParametersChangedEventHandler(ByVal o As Object, ByVal e As EventArgs)
			Me.OnDataSourceViewChanged(EventArgs.Empty)
		End Sub
		Public Overrides ReadOnly Property CanUpdate() As Boolean
			Get
				Return Not DataSource Is Nothing
			End Get
		End Property
		Public Overrides ReadOnly Property CanInsert() As Boolean
			Get
				Return True
			End Get
		End Property
		Public Overrides ReadOnly Property CanDelete() As Boolean
			Get
				Return True
			End Get
		End Property
		Public Overrides ReadOnly Property CanSort() As Boolean
			Get
				Return Not DataSource Is Nothing
			End Get
		End Property
		Public Overrides ReadOnly Property CanPage() As Boolean
			Get
				Return DataSource.EnablePaging
			End Get
		End Property
		Public Overrides ReadOnly Property CanRetrieveTotalRowCount() As Boolean
			Get
				Return Not DataSource Is Nothing
			End Get
		End Property
		Protected ReadOnly Property DataRecords() As List(Of Object)
			Get
				Return dataRecords_Renamed
			End Get
		End Property
		Public Function GetTotalCount() As Integer
			If DataSourceView Is Nothing Then
			Return 0
			End If
			If totalCount <> -1 Then
			Return totalCount
			End If
			PopulateDataSourceInfo()
			Return totalCount
		End Function
		Protected Function ExecuteViewSelect(ByVal ds As DataSourceSelectArguments, ByVal applySorting As Boolean) As IEnumerable
			If applySorting AndAlso Not sortInfo Is Nothing AndAlso sortInfo.Count > 0 Then
				ds.SortExpression = GetSortExpression(sortInfo)
			End If
			Return DataSourceView.Select(ds)
		End Function
		Private Function GetSortExpression(ByVal sortInfo As ListSortDescriptionCollection) As String
			Dim res As String = String.Empty
			For Each sort As ListSortDescription In sortInfo
				If res.Length > 0 Then
				res &= ";"
				End If
				If sort.SortDirection = ListSortDirection.Ascending Then
					res &= String.Format("{0}{1}", sort.PropertyDescriptor.Name," ASC")
				Else
					res &= String.Format("{0}{1}", sort.PropertyDescriptor.Name," DESC")
				End If

			Next sort
			Return res
		End Function
		Protected Overridable Function GetItem(ByVal index As Integer) As Object
			If (Not IsValidIndex(index)) Then
			PopulateList(index)
			End If
			If IsValidIndex(index) Then
			Return DataRecords(index - startIndex)
			End If
			Return Nothing
		End Function
		Protected Overridable Sub PopulateList(ByVal index As Integer)
			If DataSource Is Nothing Then
			Return
			End If
			DataRecords.Clear()
			Dim ds As DataSourceSelectArguments = New DataSourceSelectArguments(index, owner.PageSize)
			Dim res As IEnumerable = ExecuteViewSelect(ds, True)
			Me.startIndex = index
			For Each obj As Object In res
				DataRecords.Add(obj)
			Next obj
		End Sub
		Private Function IsValidIndex(ByVal index As Integer) As Boolean
			If index < startIndex OrElse index >= startIndex + DataRecords.Count Then
			Return False
			End If
			Return True
		End Function
		Private properties As PropertyDescriptorCollection
		Private Sub PopulateDataSourceInfo()
			If DataSourceView Is Nothing Then
			Return
			End If
			Dim ds As DataSourceSelectArguments = New DataSourceSelectArguments()
			ds.RetrieveTotalRowCount = True
			Dim data As Object = ExecuteViewSelect(ds, False)
			Me.totalCount = ds.TotalRowCount
			Dim list As ITypedList = TryCast(data, ITypedList)
			If Not list Is Nothing Then
				Dim collection As PropertyDescriptorCollection = list.GetItemProperties(Nothing)
				Me.properties = GetProperties(collection)
			Else
				Dim columns As DataColumnInfo() = New DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(data)
				Dim props As PropertyDescriptor() = New PropertyDescriptor(columns.Length - 1){}
				For n As Integer = 0 To columns.Length - 1
				props(n) = New PropertyDescriptorHelper(columns(n).PropertyDescriptor)
				Next n
				Me.properties = New PropertyDescriptorCollection(props)
			End If
		End Sub
		Private Function GetProperties(ByVal collection As PropertyDescriptorCollection) As PropertyDescriptorCollection
			Dim res As PropertyDescriptor() = New PropertyDescriptor(collection.Count - 1){}
			For n As Integer = 0 To collection.Count - 1
			res(n) = New PropertyDescriptorHelper(collection(n))
			Next n
			Return New PropertyDescriptorCollection(res)
		End Function
		Private Function GetListName(ByVal listAccessors As PropertyDescriptor()) As String Implements ITypedList.GetListName
			Return "DefaultView"
		End Function

		#region "IListServer Members"
		Private sortInfo As ListSortDescriptionCollection = Nothing
		Private Sub ApplySort(ByVal sortInfo As ListSortDescriptionCollection, ByVal groupCount As Integer, ByVal summaryInfo As List(Of ListSourceSummaryItem), ByVal totalSummaryInfo As List(Of ListSourceSummaryItem)) Implements IListServer.ApplySort
			If groupCount > 0 Then
			Throw New NotSupportedException("Grouping is not supported")
			End If
			Me.sortInfo = sortInfo
		End Sub
		Private Property FilterCriteria() As DevExpress.Data.Filtering.CriteriaOperator Implements IListServer.FilterCriteria
			Get
				Return Nothing
			End Get
			Set(ByVal value As DevExpress.Data.Filtering.CriteriaOperator)
			End Set
		End Property
		Private Function FindKeyByBeginWith(ByVal column As PropertyDescriptor, ByVal value As String) As Object Implements IListServer.FindKeyByBeginWith
			Return Nothing
		End Function
		Private Function FindKeyByValue(ByVal column As PropertyDescriptor, ByVal value As Object) As Object Implements IListServer.FindKeyByValue
			Return Nothing
		End Function
		Private Function GetGroupInfo(ByVal parentGroup As ListSourceGroupInfo) As List(Of ListSourceGroupInfo) Implements IListServer.GetGroupInfo
			Return New List(Of ListSourceGroupInfo)()
		End Function

		Private Function GetRowIndexByKey(ByVal key As Object) As Integer Implements IListServer.GetRowIndexByKey
			Return CInt(Fix(key))
		End Function
		Private Function GetRowKey(ByVal index As Integer) As Object Implements IListServer.GetRowKey
			Return index
		End Function
		Private Function GetTotalSummary() As Dictionary(Of Object, Object) Implements IListServer.GetTotalSummary
			Return Nothing
		End Function
		Private Function GetUniqueColumnValues(ByVal descriptor As PropertyDescriptor, ByVal maxCount As Integer, ByVal includeFilteredOut As Boolean, ByVal roundDataTime As Boolean) As Object() Implements IListServer.GetUniqueColumnValues
			Return Nothing
		End Function
		#End Region

		#region "IList/ICollection/IEnumerable Members"
		Private Function Add(ByVal value As Object) As Integer Implements IList.Add
			Throw New Exception("The method or operation is not implemented.")
		End Function
		Private Sub Clear() Implements IList.Clear
			Throw New Exception("The method or operation is not implemented.")
		End Sub
		Private Function Contains(ByVal value As Object) As Boolean Implements IList.Contains
			Throw New Exception("The method or operation is not implemented.")
		End Function
		Private Function IndexOf(ByVal value As Object) As Integer Implements IList.IndexOf
			Throw New Exception("The method or operation is not implemented.")
		End Function
		Private Sub Insert(ByVal index As Integer, ByVal value As Object) Implements IList.Insert
			Throw New Exception("The method or operation is not implemented.")
		End Sub
		Private ReadOnly Property IsFixedSize() As Boolean Implements IList.IsFixedSize
			Get
				Throw New Exception("The method or operation is not implemented.")
			End Get
		End Property
		Private ReadOnly Property IsReadOnly() As Boolean Implements IList.IsReadOnly
			Get
				Return CanUpdate
			End Get
		End Property
		Private Sub Remove(ByVal value As Object) Implements IList.Remove
			Throw New Exception("The method or operation is not implemented.")
		End Sub
		Private Sub RemoveAt(ByVal index As Integer) Implements IList.RemoveAt
			Throw New Exception("The method or operation is not implemented.")
		End Sub
		Public Property IList_Item(ByVal index As Integer) As Object Implements IList.Item
			Get
				Return GetItem(index)
			End Get
			Set(ByVal value As Object)
				Throw New Exception("The method or operation is not implemented.")
			End Set
		End Property
		Private Sub CopyTo(ByVal array As Array, ByVal index As Integer) Implements ICollection.CopyTo
			Throw New Exception("The method or operation is not implemented.")
		End Sub
		Private ReadOnly Property Count() As Integer Implements ICollection.Count
			Get
				Return GetTotalCount()
			End Get
		End Property
		Private ReadOnly Property IsSynchronized() As Boolean Implements ICollection.IsSynchronized
			Get
				Return True
			End Get
		End Property
		Private ReadOnly Property SyncRoot() As Object Implements ICollection.SyncRoot
			Get
				Return Me
			End Get
		End Property
		Private Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
			Return DataRecords.GetEnumerator()
		End Function
		#End Region
		#region "ITypedList Members"
		Private Function GetItemProperties(ByVal listAccessors As PropertyDescriptor()) As PropertyDescriptorCollection Implements ITypedList.GetItemProperties
			If Not properties Is Nothing Then
			Return properties
			End If
			PopulateDataSourceInfo()
			Return properties
		End Function
		#End Region
		#region "PropertyDescriptorHelper"
		Friend Class PropertyDescriptorHelper
			Inherits PropertyDescriptor
			Private source As PropertyDescriptor
			Private current As Object = Nothing
			Public Sub New(ByVal source As PropertyDescriptor)
				MyBase.New(source)
				Me.source = source
			End Sub
			Private currentDesc As PropertyDescriptor
			Private Function GetDescriptor(ByVal component As Object) As PropertyDescriptor
				If current Is component Then
				Return currentDesc
				End If
				If component Is Nothing Then
				Return source
				End If
				current = component
				Dim res As PropertyDescriptorCollection = TypeDescriptor.GetProperties(component)
				currentDesc = res(Name)
				If currentDesc Is Nothing Then
				currentDesc = source
				End If
				Return currentDesc
			End Function

			Public Overrides Function CanResetValue(ByVal component As Object) As Boolean
				Return GetDescriptor(component).CanResetValue(component)
			End Function
			Public Overrides ReadOnly Property ComponentType() As Type
				Get
					Return source.ComponentType
				End Get
			End Property
			Public Overrides Function GetValue(ByVal component As Object) As Object
				Return GetDescriptor(component).GetValue(component)
			End Function
			Public Overrides ReadOnly Property IsReadOnly() As Boolean
				Get
					Return source.IsReadOnly
				End Get
			End Property
			Public Overrides ReadOnly Property PropertyType() As Type
				Get
					Return source.PropertyType
				End Get
			End Property
			Public Overrides Sub ResetValue(ByVal component As Object)
				GetDescriptor(component).ResetValue(component)
			End Sub
			Public Overrides Sub SetValue(ByVal component As Object, ByVal value As Object)
				GetDescriptor(component).SetValue(component, value)
			End Sub
			Public Overrides Function ShouldSerializeValue(ByVal component As Object) As Boolean
				Return GetDescriptor(component).ShouldSerializeValue(component)
			End Function
		End Class
		#End Region
	End Class
End Namespace
