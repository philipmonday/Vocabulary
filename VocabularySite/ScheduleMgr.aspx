<%@ Page Title="ScheduleMgr" Language="C#" Debug="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="ScheduleMgr.aspx.cs" Inherits="ScheduleMgr" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server"> 
    <p>Unit:
        <asp:Label ID="lblUnitID" runat="server" Font-Bold="True" Font-Size="XX-Large" ForeColor="#0066FF" Text="Label"></asp:Label>
        &nbsp&nbsp
    </p>
     <div>
        <table >
            <tr>
                <td style="vertical-align: top" >
                    <asp:ListView ID="lvSchedule" runat="server" DataSourceID="SqlDataSourceSchedulePick" onitemdeleting="lvSchedule_ItemDeleting" OnDataBound="lvSchedule_DataBound">
                        <LayoutTemplate>
                            <table runat="server">
                                <tr runat="server">
                                    <td runat="server">
                                        <table ID="itemPlaceholderContainer" runat="server" border="1" style="background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;">
                                            <tr runat="server" style="background-color:#DCDCDC;color: #000000;">
                                                <th runat="server"></th>
                                                <th runat="server">ScheduleTitle</th>
                                                <th runat="server">Date</th>
                                                <th runat="server">TimeUsed</th>
                                                <th runat="server" > </th>
                                            </tr>
                                            <tr ID="itemPlaceholder" runat="server">
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr runat="server">
                                    <td runat="server" style="text-align: center;background-color: #CCCCCC;font-family: Verdana, Arial, Helvetica, sans-serif;color: #000000;">
                                    </td>
                                </tr>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr style="background-color:#DCDCDC;color: #000000;">
                                <td>
                                    <asp:Label ID="lblScheduleId" runat="server" Text='<%# Eval("Id") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="lblScheduleTitle" runat="server" Text='<%# Eval("ScheduleTitle") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="lblScheduleDate"  runat="server" Text='<%# Eval("ScheduleDate") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="lblTimeUsed"  runat="server" Text='<%# Eval("TimeUsed") %>' />
                                </td>
                                <td>
                                    <asp:Button ID="DeleteButton" runat="server" Visible='<%#Eval("Result").ToString()=="1" ?false:true  %>' CommandName="Delete" Text="Delete" />
                                </td>
                            </tr>
                        </ItemTemplate>        
                      </asp:ListView>
                    <asp:SqlDataSource ID="SqlDataSourceSchedulePick" runat="server"
                            ConnectionString="<%$ ConnectionStrings:worddbConnectionString %>"
                            ProviderName="<%$ ConnectionStrings:worddbConnectionString.ProviderName %>"
                            SelectCommand="SELECT Id, ScheduleDate, ScheduleTitle, TimeUsed, Result FROM UnitStudySchedule WHERE UnitId = @UnitId ORDER BY Id"
                            DeleteCommand="DELETE from UnitStudySchedule WHERE Id = 0"
                            CancelSelectOnNullParameter="False">
                            <SelectParameters>
                                <asp:ControlParameter Name="UnitId" ControlID = "lblUnitID" PropertyName="Text" />
                            </SelectParameters>
                    </asp:SqlDataSource>
                    <br />
                    New Schedule Title:
                    <br />
                    <asp:TextBox ID="txtScheduleTitle" runat="server" Width="172px" Height="27px" MaxLength="200" ></asp:TextBox>
                    <br />
                    Select Date:
                    <br />
                    <div>
                        <asp:Panel ID="Panel1" runat="server" Height="27px" Width="253px">
                            <asp:TextBox ID="txtScheduleDate" runat="server" Width ="172px"></asp:TextBox>
                            &nbsp;&nbsp;&nbsp;
                            <asp:Button ID="Button1" runat="server" OnClick="btnOnClick" Text="选择" />
                            <div id="LayerCC" style="left: 155px; width: 189px; position: absolute; top: 55px;
                                height: 191px; background-color: white" visible="false" runat="server">
                                <asp:Calendar ID="Calendar1" runat="server" BackColor="White" BorderColor="#3366CC"
                                    BorderWidth="1px" CellPadding="1" Font-Names="Verdana" Font-Size="8pt" ForeColor="#003399"
                                    Height="200px" OnSelectionChanged="Calendar1_SelectionChanged" Width="220px">
                                    <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                                    <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
                                    <WeekendDayStyle BackColor="#CCCCFF" />
                                    <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
                                    <OtherMonthDayStyle ForeColor="#999999" />
                                    <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
                                    <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
                                    <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True"
                                        Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
                                </asp:Calendar>
                            </div>
                        </asp:Panel>
                    </div>
                    <br />
                    <asp:Button ID="btnAddSchedule" runat="server" OnClick="btnAddSchedule_Click" Text="NewSchedule" />
                     &nbsp&nbsp
                    <asp:Button ID="btnMain" runat="server" OnClick="btnMain_Click" Text="Main" Width="75px"/>
                    <br />
                </td>
            </tr>
            <tr>
                <td>
                    Review Frequency:
                    <asp:Label ID="lblReviewFrequency" runat="server" ></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    
</asp:Content>
