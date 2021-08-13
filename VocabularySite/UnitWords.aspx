<%@ Page Title="UnitWords"Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="UnitWords.aspx.cs" Inherits="UnitWords" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Words In Unit  <asp:Label ID ="lblCurrentUnitID" runat="server"></asp:Label></h3>
    <p>Learn the words in Unit</p>

    <div>
        <table >
            <tr>
                <td style="vertical-align: top">
                        <asp:ListBox ID="ListBox1" runat="server" DataSourceID="SqlDataSourceWordPick" DataTextField="WordBody" DataValueField="Id" Height="400px" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="ListBox1_SelectedIndexChanged">
                        </asp:ListBox>
                        <asp:SqlDataSource ID="SqlDataSourceWordPick" runat="server"
                                ConnectionString="<%$ ConnectionStrings:worddbConnectionString %>"
                                ProviderName="<%$ ConnectionStrings:worddbConnectionString.ProviderName %>"
                                SelectCommand="SELECT Id, WordBody FROM Word WHERE UnitId = @UnitId"
                                CancelSelectOnNullParameter="false">
                                <SelectParameters>
                                    <asp:SessionParameter Name="UnitId" SessionField="SelectedScheduleUnitID" />
                                </SelectParameters>
                         </asp:SqlDataSource>
                </td>
                <td >  <asp:Label ID="lblspan1" runat="server"  Width ="20px"></asp:Label> </td>
                <td style="vertical-align: top" >
                    <p>
                    <asp:Label ID="lblWordBody" runat="server" Width="200px" Font-Bold="True" Font-Size="XX-Large" ForeColor="#0066FF" Text=""></asp:Label>
                    <br />
                    <asp:Label ID="lblDesc1" runat="server" Font-Size="Small" ForeColor="#0066FF" Text="" ></asp:Label>
                    <br />
                    <asp:Label ID="lblDesc2" runat="server" Font-Size="Small" ForeColor="#0066FF" Text="" ></asp:Label>
                    <br />
                    <asp:Label ID="lblDesc3" runat="server" Font-Size="Small" ForeColor="#0066FF" Text="" ></asp:Label>
                    <br />
                    <asp:Label ID="lblDesc4" runat="server" Font-Size="Small" ForeColor="#0066FF" Text="" ></asp:Label>
                    <br />
                    <asp:Label ID="lblDesc5" runat="server" Font-Size="Small" ForeColor="#0066FF" Text="" ></asp:Label>
                    <br />
                    <asp:Label ID="lblDesc6" runat="server" Font-Size="Small" ForeColor="#0066FF" Text="" ></asp:Label>
                    <br />
                    </p>
                </td>
                <td >  <asp:Label ID="lblspan2" runat="server"  Width ="20px"></asp:Label> </td>
                <td style="vertical-align: top" >
                    <asp:ListView ID="lvHints" runat="server" DataSourceID="SqlDataSourceHintPick" onitemdeleting="lvHints_ItemDeleting">
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
                                    <asp:ControlParameter Name="WordId" ControlID = "ListBox1" PropertyName="SelectedItem.Value" />
                                </SelectParameters>

                      </asp:SqlDataSource>
                    <br />
                    <asp:TextBox ID="txtNewHint" runat="server" Width="200px" Height="76px" TextMode="MultiLine" MaxLength="200"></asp:TextBox>
                    <br />
                    <asp:Button ID="btnAddHint" runat="server" OnClick="btnAddHint_Click" Text="NewHint" />
                </td>
            </tr>
        </table>
    </div>
    <p></p> 
    <div>
        <asp:Button ID="btnMain" runat="server" OnClick="btnMain_Click" Text="Main" Width="75px"/>
    </div>

</asp:Content>
