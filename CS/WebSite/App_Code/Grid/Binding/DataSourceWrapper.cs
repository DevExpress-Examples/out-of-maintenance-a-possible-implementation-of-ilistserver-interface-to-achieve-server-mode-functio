using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Security.Permissions;
using System.ComponentModel;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Data;
using System.Collections.Generic;

namespace DevExpress.Web {
    [PersistChildren(false), ParseChildren(true),
    DisplayName("ObjectDataSource wrapper"),
    AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ObjectDataSourceWrapper : DataSourceControl {
        string dataSourceId = string.Empty;
        ObjectDataSource dataSource = null;
        int pageSize = 10;
        public ObjectDataSourceWrapper() { }
        protected override DataSourceView GetView(string viewName) {
            if((viewName != null) && ((viewName.Length == 0) || string.Equals(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase))) {
                return this.GetView();
            }
            throw new ArgumentException("", "viewName");
        }

        public ObjectDataSource DataSource {
            get { return dataSource; }
            set { dataSource = value; }
        }
        [DefaultValue(10)]
        public int PageSize {
            get { return pageSize; }
            set {
                if(value < 1) value = 1;
                pageSize = value;
            }
        }
        [DefaultValue(""), Themeable(false), Category("Data"), AutoFormatDisable, Localizable(false)]
        public virtual string DataSourceID {
            get { return dataSourceId; }
            set { dataSourceId = value; }
        }
        ObjectDataSourceWrapperView _view;
        ObjectDataSourceWrapperView GetView() {
            if(this._view == null) {
                this._view = new ObjectDataSourceWrapperView(this, "DefaultView", this.Context);
            }
            return this._view;
        }
        protected override ICollection GetViewNames() { return new string[] { "DefaultView" }; }
        protected internal virtual ObjectDataSource GetDataSource() {
            if(DataSource != null) return DataSource;
            if(!String.IsNullOrEmpty(DataSourceID)) return DataControlHelper.FindControl(this, DataSourceID) as ObjectDataSource;
            return null;
        }
    }
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class ObjectDataSourceWrapperView : DataSourceView, IListServer, ITypedList {
        ObjectDataSourceWrapper owner;
        HttpContext context;
        ObjectDataSource dataSource;
        ObjectDataSourceView dataSourceView;
        List<object> dataRecords;
        int startIndex = 0, totalCount = -1;
        public ObjectDataSourceWrapperView(ObjectDataSourceWrapper owner, string name, HttpContext context)
            : base(owner, name) {
            this.owner = owner;
            this.context = context;
            this.dataRecords = new List<object>();
            this.dataSource = owner.GetDataSource();
        }

        protected ObjectDataSource DataSource { get { return dataSource; } }
        protected ObjectDataSourceView DataSourceView { get { return dataSourceView; } }
        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments) {
            if(this.dataSource != null) {
                this.dataSourceView = ((IDataSource)DataSource).GetView("DefaultView") as ObjectDataSourceView;
            }
            return this;
        }
        protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues) {
            if(DataSourceView == null)
                throw new NotSupportedException();
            return DataSourceView.Update(keys, values, oldValues);
        }
        protected override int ExecuteInsert(IDictionary values) {
            if(DataSourceView == null)
                throw new NotSupportedException();
            return DataSourceView.Insert(values);
        }

        protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues) {
            if(DataSourceView == null)
                throw new NotSupportedException();
            return DataSourceView.Delete(keys, oldValues);
        }
        void CriteriaParametersChangedEventHandler(object o, EventArgs e) {
            this.OnDataSourceViewChanged(EventArgs.Empty);
        }
        public override bool CanUpdate { get { return DataSource != null; } }
        public override bool CanInsert { get { return true; } }
        public override bool CanDelete { get { return true; } }
        public override bool CanSort { get { return DataSource != null; } }
        public override bool CanPage { get { return DataSource.EnablePaging; } }
        public override bool CanRetrieveTotalRowCount {
            get {
                return DataSource != null;
            }
        }
        protected List<object> DataRecords { get { return dataRecords; } }
        public int GetTotalCount() {
            if(DataSourceView == null) return 0;
            if(totalCount != -1) return totalCount;
            PopulateDataSourceInfo();
            return totalCount;
        }
        protected IEnumerable ExecuteViewSelect(DataSourceSelectArguments ds, bool applySorting) {
            if(applySorting && sortInfo != null && sortInfo.Count > 0) {
                ds.SortExpression = GetSortExpression(sortInfo);
            }
            return DataSourceView.Select(ds);
        }
        string GetSortExpression(ListSortDescriptionCollection sortInfo) {
            string res = string.Empty;
            foreach(ListSortDescription sort in sortInfo) {
                if(res.Length > 0) res += ";";
                res += string.Format("{0}{1}", sort.PropertyDescriptor.Name, sort.SortDirection == ListSortDirection.Ascending ? " ASC" : " DESC");

            }
            return res;
        }
        protected virtual object GetItem(int index) {
            if(!IsValidIndex(index)) PopulateList(index);
            if(IsValidIndex(index)) return DataRecords[index - startIndex];
            return null;
        }
        protected virtual void PopulateList(int index) {
            if(DataSource == null) return;
            DataRecords.Clear();
            DataSourceSelectArguments ds = new DataSourceSelectArguments(index, owner.PageSize);
            IEnumerable res = ExecuteViewSelect(ds, true);
            this.startIndex = index;
            foreach(object obj in res) {
                DataRecords.Add(obj);
            }
        }
        bool IsValidIndex(int index) {
            if(index < startIndex || index >= startIndex + DataRecords.Count) return false;
            return true;
        }
        PropertyDescriptorCollection properties;
        void PopulateDataSourceInfo() {
            if(DataSourceView == null) return;
            DataSourceSelectArguments ds = new DataSourceSelectArguments();
            ds.RetrieveTotalRowCount = true;
            object data = ExecuteViewSelect(ds, false);
            this.totalCount = ds.TotalRowCount;
            ITypedList list = data as ITypedList;
            if(list != null) {
                PropertyDescriptorCollection collection = list.GetItemProperties(null);
                this.properties = GetProperties(collection);
            }
            else {
                DataColumnInfo[] columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(data);
                PropertyDescriptor[] props = new PropertyDescriptor[columns.Length];
                for(int n = 0; n < columns.Length; n++) props[n] = new PropertyDescriptorHelper(columns[n].PropertyDescriptor);
                this.properties = new PropertyDescriptorCollection(props);
            }
        }
        PropertyDescriptorCollection GetProperties(PropertyDescriptorCollection collection) {
            PropertyDescriptor[] res = new PropertyDescriptor[collection.Count];
            for(int n = 0; n < collection.Count; n++) res[n] = new PropertyDescriptorHelper(collection[n]);
            return new PropertyDescriptorCollection(res);
        }
        string ITypedList.GetListName(PropertyDescriptor[] listAccessors) { return "DefaultView"; }

        #region //IListServer Members
        ListSortDescriptionCollection sortInfo = null;
        void IListServer.ApplySort(ListSortDescriptionCollection sortInfo, int groupCount, List<ListSourceSummaryItem> summaryInfo, List<ListSourceSummaryItem> totalSummaryInfo) {
            if(groupCount > 0) throw new NotSupportedException("Grouping is not supported");
            this.sortInfo = sortInfo;    
        }
        DevExpress.Data.Filtering.CriteriaOperator IListServer.FilterCriteria {
            get { return null; }
            set {  }
        }
        int IListServer.FindIncremental(PropertyDescriptor column, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
            return -1;
        }
        int IListServer.FindIncrementalByValue(PropertyDescriptor column, object value, int startIndex, bool searchUp) {
            return -1;
        }

        object IListServer.FindKeyByBeginWith(PropertyDescriptor column, string value) { return null; }
        object IListServer.FindKeyByValue(PropertyDescriptor column, object value) { return null; }
        List<ListSourceGroupInfo> IListServer.GetGroupInfo(ListSourceGroupInfo parentGroup) {
            return new List<ListSourceGroupInfo>(); 
        }

        int IListServer.GetRowIndexByKey(object key) { return (int)key; }
        object IListServer.GetRowKey(int index) { return index; }
        Dictionary<object, object> IListServer.GetTotalSummary() { return null; }
        object[] IListServer.GetUniqueColumnValues(PropertyDescriptor descriptor, int maxCount, bool includeFilteredOut, bool roundDataTime) { return null; }
        IList IListServer.GetAllFilteredAndSortedRows() {
            return null;
        }
        #endregion

        #region //IList/ICollection/IEnumerable Members
        int IList.Add(object value) {
            throw new Exception("The method or operation is not implemented.");
        }
        void IList.Clear() { throw new Exception("The method or operation is not implemented."); }
        bool IList.Contains(object value) { throw new Exception("The method or operation is not implemented."); }
        int IList.IndexOf(object value) { throw new Exception("The method or operation is not implemented."); }
        void IList.Insert(int index, object value) { throw new Exception("The method or operation is not implemented."); }
        bool IList.IsFixedSize { get { throw new Exception("The method or operation is not implemented."); } }
        bool IList.IsReadOnly { get { return CanUpdate; } }
        void IList.Remove(object value) { throw new Exception("The method or operation is not implemented."); }
        void IList.RemoveAt(int index) { throw new Exception("The method or operation is not implemented."); }
        object IList.this[int index] {
            get { return GetItem(index); }
            set { throw new Exception("The method or operation is not implemented."); }
        }
        void ICollection.CopyTo(Array array, int index) { throw new Exception("The method or operation is not implemented."); }
        int ICollection.Count { get { return GetTotalCount(); } }
        bool ICollection.IsSynchronized { get { return true; } }
        object ICollection.SyncRoot { get { return this; } }
        IEnumerator IEnumerable.GetEnumerator() { return DataRecords.GetEnumerator(); }
        #endregion
        #region //ITypedList Members
        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
            if(properties != null) return properties;
            PopulateDataSourceInfo();
            return properties;
        }
        #endregion
        #region //PropertyDescriptorHelper
        internal class PropertyDescriptorHelper : PropertyDescriptor {
            PropertyDescriptor source;
            object current = null;
            public PropertyDescriptorHelper(PropertyDescriptor source)
                : base(source) {
                this.source = source;
            }
            PropertyDescriptor currentDesc;
            PropertyDescriptor GetDescriptor(object component) {
                if(current == component) return currentDesc;
                if(component == null) return source;
                current = component;
                PropertyDescriptorCollection res = TypeDescriptor.GetProperties(component);
                currentDesc = res[Name];
                if(currentDesc == null) currentDesc = source;
                return currentDesc;
            }

            public override bool CanResetValue(object component) {
                return GetDescriptor(component).CanResetValue(component);
            }
            public override Type ComponentType { get { return source.ComponentType; } }
            public override object GetValue(object component) { return GetDescriptor(component).GetValue(component); }
            public override bool IsReadOnly { get { return source.IsReadOnly; } }
            public override Type PropertyType { get { return source.PropertyType; } }
            public override void ResetValue(object component) {
                GetDescriptor(component).ResetValue(component);
            }
            public override void SetValue(object component, object value) {
                GetDescriptor(component).SetValue(component, value);
            }
            public override bool ShouldSerializeValue(object component) {
                return GetDescriptor(component).ShouldSerializeValue(component);
            }
        }
        #endregion
    }
}
