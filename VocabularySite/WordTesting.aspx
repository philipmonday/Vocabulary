<%@ Page Title="WordTesting" Language="C#"  Debug="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="WordTesting.aspx.cs" Inherits="WordTesting" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <asp:Label ID="lblQuizBody" runat="server" Font-Bold="True" Font-Size="XX-Large" ForeColor="#0066FF" Text="Label"></asp:Label>
        &nbsp&nbsp
        <asp:Label ID="lblWordId" runat="server" Text="WordId" Visible ="true"></asp:Label>
        <asp:Label ID="lblCorrectOptionIndex" runat="server" Text="Label" Visible ="false"></asp:Label> 
    </p>
    <p>
         <asp:RadioButtonList ID="rblOptions" runat="server"></asp:RadioButtonList>
    </p>
    <p>
        <asp:Label ID="lblSubmitResult" runat="server" Font-Size="Medium" Font-Bold="True" ForeColor="#0066FF" Text="  "></asp:Label>
    </p>
    <p>
         <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" Width="75px" />
         &nbsp&nbsp
         <asp:Button ID="btnNext" runat="server" Text="Next" enabled="false" OnClick="btnNext_Click" Width="75px"/>
         &nbsp&nbsp
         <asp:Button ID="btnShowHint" runat="server" Text="Hint"  OnClick="btnShowHint_Click" Width="75px"/>
          &nbsp&nbsp
         <asp:Button ID="btnMain" runat="server" OnClick="btnMain_Click" Text="Main" Width="75px"/>
     </p>

    <div>
                      <asp:ListView ID="lvHints" runat="server" visible="false"  DataSourceID="SqlDataSourceHintPick" onitemdeleting="lvHints_ItemDeleting">
                        <LayoutTemplate>
                            <table runat="server">
                                <tr runat="server">
                                    <td runat="server">
                                        <table ID="itemPlaceholderContainer" runat="server" border="1" style="background-color: #FFFFFF;border-collapse: collapse;border-color: #999999;border-style:none;border-width:1px;font-family: Verdana, Arial, Helvetica, sans-serif;">
                                            <tr runat="server" style="background-color:#DCDCDC;color: #000000;">
                                                <th runat="server"></th>
                                                <th runat="server">Hints</th>
                                                <th runat="server"> </th>
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
                                    <asp:Label ID="lblHintId" runat="server" Text='<%# Eval("Id") %>' />
                                </td>
                                <td>
                                    <asp:Label ID="lblHint" runat="server" Text='<%# Eval("Hint") %>' />
                                </td>
                                <td>
                                    <asp:Button ID="DeleteButton" runat="server" CommandName="Delete" Text="Delete" />
                                </td>
                            </tr>
                        </ItemTemplate>        
                      </asp:ListView>
                      <asp:SqlDataSource ID="SqlDataSourceHintPick" runat="server"
                                ConnectionString="<%$ ConnectionStrings:worddbConnectionString %>"
                                ProviderName="<%$ ConnectionStrings:worddbConnectionString.ProviderName %>"
                                SelectCommand="SELECT Id, Hint FROM WordHint WHERE WordId = @WordId"
                                DeleteCommand="DELETE from WordHint WHERE Id = 0"
                                CancelSelectOnNullParameter="False">
                                <SelectParameters>
                                    <asp:ControlParameter Name="WordId" ControlID="lblWordId" PropertyName="Text"/>
                                </SelectParameters>
                      </asp:SqlDataSource>
                      <br />
                      <asp:TextBox ID="txtNewHint" runat="server" visible="false" Width="200px" Height="76px" TextMode="MultiLine" MaxLength="200"></asp:TextBox>
                      <br />
                      <asp:Button ID="btnAddHint" runat="server" visible="false" OnClick="btnAddHint_Click" Text="NewHint" />
    </div>
     <div>
         <asp:Table ID="InspectingTable" BorderStyle ="Solid" BorderWidth="5" runat="server" visible ="true" ></asp:Table>
         <br />
         Training Result:0-wrong 1-correct 2-withHint 
         <br />
         <asp:Label ID="lblTrainingResult" runat="server" Text="" Visible="true"></asp:Label>
     </div>
 
     <div>
         <asp:Table ID="HolderTable" BorderStyle ="Solid" BorderWidth="5" runat="server" visible ="false" ></asp:Table>
    </div>
</asp:Content>