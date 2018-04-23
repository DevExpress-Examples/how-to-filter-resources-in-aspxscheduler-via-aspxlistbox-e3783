Imports System
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports DevExpress.Web.ASPxScheduler
Imports DevExpress.XtraScheduler

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Private lastInsertedAppointmentId As Integer

    Private defaultResourcesSelectCommand As String = "SELECT [ID], [Model] FROM [Cars]"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim param As String = Page.Request("__CALLBACKPARAM")

        If Not IsPostBack Then
            Session("resourcesSelectCommand") = defaultResourcesSelectCommand
        End If

        If (Not String.IsNullOrEmpty(param)) AndAlso param.Contains("FLTRES|") Then
            Dim resources As String = param.Substring(param.IndexOf("|"c) + 1)
            If resources.Length > 0 Then
                Session("resourcesSelectCommand") = String.Format("SELECT [ID], [Model] FROM [Cars] WHERE [ID] IN ({0})", resources)
            Else
                Session("resourcesSelectCommand") = defaultResourcesSelectCommand
            End If
        End If

        SqlDataSourceResources.SelectCommand = Session("resourcesSelectCommand").ToString()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsPostBack Then
            ASPxScheduler1.Start = New Date(2008, 7, 12)
        End If
    End Sub

    Protected Sub ASPxScheduler1_BeforeExecuteCallbackCommand(ByVal sender As Object, ByVal e As SchedulerCallbackCommandEventArgs)
        If e.CommandId.Contains("FLTRES") Then
            ASPxScheduler1.ApplyChanges(ASPxSchedulerChangeAction.NotifyResourceIntervalChanged)
        End If
    End Sub

    Protected Sub ASPxScheduler1_AppointmentRowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxScheduler.ASPxSchedulerDataInsertingEventArgs)
        e.NewValues.Remove("ID")
    End Sub

    Protected Sub SqlDataSourceAppointments_Inserted(ByVal sender As Object, ByVal e As SqlDataSourceStatusEventArgs)
        Dim connection As SqlConnection = CType(e.Command.Connection, SqlConnection)

        Using command As New SqlCommand("SELECT IDENT_CURRENT('CarScheduling')", connection)
            lastInsertedAppointmentId = Convert.ToInt32(command.ExecuteScalar())
        End Using
    End Sub

    Protected Sub ASPxScheduler1_AppointmentRowInserted(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxScheduler.ASPxSchedulerDataInsertedEventArgs)
        e.KeyFieldValue = lastInsertedAppointmentId
    End Sub

    Protected Sub ASPxScheduler1_AppointmentsInserted(ByVal sender As Object, ByVal e As DevExpress.XtraScheduler.PersistentObjectsEventArgs)
        Dim apt As Appointment = CType(e.Objects(0), Appointment)
        Dim storage As ASPxSchedulerStorage = DirectCast(sender, ASPxSchedulerStorage)
        storage.SetAppointmentId(apt, lastInsertedAppointmentId)
    End Sub
End Class