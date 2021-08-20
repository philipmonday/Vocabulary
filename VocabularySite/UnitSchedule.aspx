<%@ Page Title="UnitSchedule" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="UnitSchedule.aspx.cs" Inherits="UnitSchedule" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>Vocabulary Test Recorder.</h3>
    <p></p>

    <div>
        <table>
            <tr>
                <td style="vertical-align: top" >
                Calendar
                </td>
                <td style="vertical-align: top" >
                Todo
                </td>
                <td style="vertical-align: top" >
                Done
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top" >
                     <asp:Calendar ID="Calendar1" runat="server" SelectedDate="<%# DateTime.Today %>"  BackColor="White" BorderColor="#3366CC" BorderWidth="1px" CellPadding="1" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="#003399" Height="200px" Width="220px" OnSelectionChanged="Calendar1_SelectionChanged">
                        <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                        <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                        <OtherMonthDayStyle ForeColor="#999999" />
                        <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                        <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                        <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True" Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                        <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                        <WeekendDayStyle BackColor="#CCCCFF" />
                    </asp:Calendar>
                </td>
                <td style="vertical-align: top">
                        <asp:ListBox ID="lbTodo" runat="server" DataSourceID="SqlDataSourceDatePick" DataTextField="UnitId" DataValueField="Id" Height="200px" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="lbTodo_SelectedIndexChanged"></asp:ListBox>
                        <asp:SqlDataSource ID="SqlDataSourceDatePick" runat="server"
                                ConnectionString="<%$ ConnectionStrings:worddbConnectionString %>"
                                ProviderName="<%$ ConnectionStrings:worddbConnectionString.ProviderName %>"
                                SelectCommand="SELECT Id, UnitId  FROM UnitStudySchedule WHERE UnitStudySchedule.ScheduleDate = @datefilter AND Result = 0"
                                CancelSelectOnNullParameter="false">
                                <SelectParameters>
                                     <asp:ControlParameter Name="datefilter" ControlID="Calendar1" PropertyName="SelectedDate" />
                                </SelectParameters>
                         </asp:SqlDataSource>
                </td>
                <td style="vertical-align: top">
                        <asp:ListBox ID="lbDone" runat="server" DataSourceID="SqlDataSourceDatePick2" DataTextField="UnitId" DataValueField="Id" Height="200px" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="lbDone_SelectedIndexChanged" ></asp:ListBox>
                        <asp:SqlDataSource ID="SqlDataSourceDatePick2" runat="server"
                                ConnectionString="<%$ ConnectionStrings:worddbConnectionString %>"
                                ProviderName="<%$ ConnectionStrings:worddbConnectionString.ProviderName %>"
                                SelectCommand="SELECT Id, UnitId  FROM UnitStudySchedule WHERE UnitStudySchedule.ScheduleDate = @datefilter AND Result = 1"
                                CancelSelectOnNullParameter="false">
                                <SelectParameters>
                                     <asp:ControlParameter Name="datefilter" ControlID="Calendar1" PropertyName="SelectedDate" />
                                </SelectParameters>
                         </asp:SqlDataSource>
                </td>
            </tr>
        </table>
    </div>
    <p></p>
    <div>
        Schedule Selected:<asp:Label ID ="lblSelectedScheduleID" runat="server"></asp:Label>
        <br />
        Schedule Selected UnitId: <asp:Label ID ="lblSelectedScheduleUnitID" runat="server"></asp:Label>
        <asp:Table ID="HolderTable" BorderStyle ="Solid" BorderWidth="5" runat="server" ViewStateMode="Disabled" ></asp:Table>
    </div>
    <p></p>
    <div>
        <asp:Button ID="btnViewWords" runat="server" OnClick="btnViewWords_Click" Text="ViewWords" />
        &nbsp&nbsp
        <asp:Button ID="btnTraining" runat="server" OnClick="btnTraining_Click" Text="Training" />
        &nbsp&nbsp
        <asp:Button ID="btnTesting" runat="server" OnClick="btnTesting_Click" Text="Testing" />
         &nbsp&nbsp
        <asp:Button ID="btnSynonyms" runat="server" OnClick="btnSynonyms_Click" Text="Synonyms" />

               &nbsp&nbsp
        <asp:Button ID="btnScheduleMgr" runat="server" OnClick="btnScheduleMgr_Click" Text="ScheduleMgr" />

    </div>

</asp:Content>
