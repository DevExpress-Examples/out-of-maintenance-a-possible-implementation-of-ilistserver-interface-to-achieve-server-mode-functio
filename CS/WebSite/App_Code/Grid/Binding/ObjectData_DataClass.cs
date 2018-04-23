using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
    public class MyDataClass {
        public DataView GetMyData(int StartRow, int PageSize) {
            return GetData(StartRow, PageSize);
        }
        private DataView GetData(int StartRow, int PageSize) {
            DataSet Contacts = new DataSet();
            Contacts.ReadXml(HttpContext.Current.Server.MapPath("~/App_Data/ContactsList.xml"));

            DataTable ContactsTable = Contacts.Tables[0];
            DataTable PagedContactsTable = ContactsTable.Clone();

            for(int i = StartRow; i < StartRow + PageSize && i < ContactsTable.Rows.Count; i++) {
                PagedContactsTable.ImportRow(ContactsTable.Rows[i]);
            }

            return PagedContactsTable.DefaultView;
        }
        public int GetRowsCount() {
            DataSet Contacts = new DataSet();
            Contacts.ReadXml(HttpContext.Current.Server.MapPath("~/App_Data/ContactsList.xml"));
            return Contacts.Tables[0].Rows.Count;
        }
    }


