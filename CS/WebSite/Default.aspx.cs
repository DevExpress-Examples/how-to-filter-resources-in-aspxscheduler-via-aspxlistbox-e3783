using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;

public partial class _Default : System.Web.UI.Page {
    private int lastInsertedAppointmentId;

    private string defaultResourcesSelectCommand = "SELECT [ID], [Model] FROM [Cars]";

    protected void Page_Init(object sender, EventArgs e) {
        string param = Page.Request["__CALLBACKPARAM"];

        if (!IsPostBack)
            Session["resourcesSelectCommand"] = defaultResourcesSelectCommand;

        if (!string.IsNullOrEmpty(param) && param.Contains("FLTRES|")) {
            string resources = param.Substring(param.IndexOf('|') + 1);
            if (resources.Length > 0)
                Session["resourcesSelectCommand"] = string.Format("SELECT [ID], [Model] FROM [Cars] WHERE [ID] IN ({0})", resources);
            else
                Session["resourcesSelectCommand"] = defaultResourcesSelectCommand;
        }

        SqlDataSourceResources.SelectCommand = Session["resourcesSelectCommand"].ToString();
    }

    protected void Page_Load(object sender, EventArgs e) {
        if (!IsPostBack) {
            ASPxScheduler1.Start = new DateTime(2008, 7, 12);
        }
    }

    protected void ASPxScheduler1_BeforeExecuteCallbackCommand(object sender, SchedulerCallbackCommandEventArgs e) {
        if (e.CommandId.Contains("FLTRES"))
            ASPxScheduler1.ApplyChanges(ASPxSchedulerChangeAction.NotifyResourceIntervalChanged);
    }

    protected void ASPxScheduler1_AppointmentRowInserting(object sender, DevExpress.Web.ASPxScheduler.ASPxSchedulerDataInsertingEventArgs e) {
        e.NewValues.Remove("ID");
    }

    protected void SqlDataSourceAppointments_Inserted(object sender, SqlDataSourceStatusEventArgs e) {
        SqlConnection connection = (SqlConnection)e.Command.Connection;

        using(SqlCommand command = new SqlCommand("SELECT IDENT_CURRENT('CarScheduling')", connection)) {
            lastInsertedAppointmentId = Convert.ToInt32(command.ExecuteScalar());
        }
    }

    protected void ASPxScheduler1_AppointmentRowInserted(object sender, DevExpress.Web.ASPxScheduler.ASPxSchedulerDataInsertedEventArgs e) {
        e.KeyFieldValue = lastInsertedAppointmentId;
    }

    protected void ASPxScheduler1_AppointmentsInserted(object sender, DevExpress.XtraScheduler.PersistentObjectsEventArgs e) {
        Appointment apt = (Appointment)e.Objects[0];
        ASPxSchedulerStorage storage = (ASPxSchedulerStorage)sender;
        storage.SetAppointmentId(apt, lastInsertedAppointmentId);
    }
}