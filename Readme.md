<!-- default file list -->
*Files to look at*:

* [DataSourceWrapper.cs](./CS/WebSite/App_Code/Grid/Binding/DataSourceWrapper.cs) (VB: [DataSourceWrapper.vb](./VB/WebSite/App_Code/Grid/Binding/DataSourceWrapper.vb))
* [ObjectData_DataClass.cs](./CS/WebSite/App_Code/Grid/Binding/ObjectData_DataClass.cs) (VB: [ObjectData_DataClass.vb](./VB/WebSite/App_Code/Grid/Binding/ObjectData_DataClass.vb))
* [Default.aspx](./CS/WebSite/Default.aspx) (VB: [Default.aspx.vb](./VB/WebSite/Default.aspx.vb))
* [Default.aspx.cs](./CS/WebSite/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/WebSite/Default.aspx.vb))
<!-- default file list end -->
# A possible implementation of IListServer interface to achieve Server Mode functionality


<p>The main aim of the sample is to demonstrate one of possible ways to implement Server Mode by implementing the IListServer interface.<br />
Because of changes in IListServer implementation, this example has different solutions for different DevExpress versions.<br />
<strong><br />
See also:</strong><br />
<a href="https://www.devexpress.com/Support/Center/p/E2672">Bind a grid to a ObjectDataSource with EnablePaging </a><u><br />
</u><a href="https://www.devexpress.com/Support/Center/p/E3027">OBSOLETE: A possible implementation of IListServer interface to achieve Server Mode functionality in the GridView extension</a></p>


<h3>Description</h3>

<p>This example demonstrates how you can bind <strong>ASPxGridView</strong> to <strong>ObjectDataSource</strong> with <strong>EnablePaging = true</strong>.</p><p>Note: Filtering, grouping and summary are not supported. In ASPxGridView 8.1.4 we implemented the <a href="http://documentation.devexpress.com/#AspNet/DevExpressWebASPxGridViewASPxGridView_DataSourceForceStandardPagingtopic">DataSourceForceStandardPaging</a> property. If it is set to true, the ASPxGridView uses the data source&#39;s ability to fetch rows for the current page only and sort data. So, there is no need to use this example code if you are using ASPxGridView 8.1.4 and higher.</p>

<br/>


