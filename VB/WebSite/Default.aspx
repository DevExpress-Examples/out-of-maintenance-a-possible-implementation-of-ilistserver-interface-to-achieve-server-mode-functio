<%-- BeginRegion Page setup --%>
<%@ Page Language="vb" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Grid_Bind_ObjectDataSourceBinding" %>
<%@ Register Assembly="DevExpress.Web.ASPxEditors.v7.3" Namespace="DevExpress.Web.ASPxEditors"  TagPrefix="dxe" %>
<%@ Register Assembly="DevExpress.Web.ASPxGridView.v7.3" Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dxwgv" %>
<%@ Register Assembly="DevExpress.Web.v7.3" Namespace="DevExpress.Web.ASPxPager" TagPrefix="dxwp" %>

<%@ Register Namespace="DevExpress.Web"
	TagPrefix="dxDataWrapper" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%-- EndRegion --%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>Bind a grid to a ObjectDataSource with EnablePaging</title>
</head>
<body>
	<form id="form1" runat="server">

	<!-- Use ObjectDataSourceWrapper instead of ObjectDataSource as Grid DataSource -->
	<dxwgv:ASPxGridView ID="grid" ClientInstanceName="grid" 
	   DataSourceID="objectDSwrapper" runat="server" Width="300px" AutoGenerateColumns="True">
	   <SettingsPager PageSize="5" />
	</dxwgv:ASPxGridView>
	<dxDataWrapper:ObjectDataSourceWrapper ID="objectDSwrapper" runat="server" DataSourceID="objectDS" PageSize="5"/>
	<asp:ObjectDataSource ID="objectDS" runat="server" SelectMethod="GetMyData"
		TypeName="MyDataClass" EnablePaging="True"
		MaximumRowsParameterName="PageSize" 
		StartRowIndexParameterName="StartRow" 
		SelectCountMethod="GetRowsCount">
	</asp:ObjectDataSource>
	</form>
</body>
</html>
