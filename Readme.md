<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128532386/10.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E166)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [DataObject.cs](./CS/WebSite/App_Code/DataObject.cs) (VB: [DataObject.vb](./VB/WebSite/App_Code/DataObject.vb))
* [Default.aspx](./CS/WebSite/Default.aspx) (VB: [Default.aspx](./VB/WebSite/Default.aspx))
* [Default.aspx.cs](./CS/WebSite/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/WebSite/Default.aspx.vb))
<!-- default file list end -->
# A possible implementation of IListServer interface to achieve Server Mode functionality
<!-- run online -->
**[[Run Online]](https://codecentral.devexpress.com/e166/)**
<!-- run online end -->


<p>The main aim of the sample is to demonstrate one of possible ways to implement Server Mode by implementing the IListServer interface.<br />
Because of changes in IListServer implementation, this example has different solutions for different DevExpress versions.<br />
<strong><br />
See also:</strong><br />
<a href="https://www.devexpress.com/Support/Center/p/E2672">Bind a grid to a ObjectDataSource with EnablePaging </a><u><br />
</u><a href="https://www.devexpress.com/Support/Center/p/E3027">OBSOLETE: A possible implementation of IListServer interface to achieve Server Mode functionality in the GridView extension</a></p>


<h3>Description</h3>

<p>Because of some breaking changes in the Data Library (<a href="http://www.devexpress.com/Support/WhatsNew/DXperience/files/10.2.3.bc.xml#autolist8">Breaking Changes in v2010 vol 2</a>) previous implementations of IListServer interface have become broken.<br />
This example demonstrates the implementation of IListServer interface using LINQ queries with LinqToSql provider.<br />
With LINQ it becomes a bit easier to implement paging and sorting operations on the database server side.</p>

<br/>


