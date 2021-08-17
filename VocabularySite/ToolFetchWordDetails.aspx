<%@ Page Title="ToolFetchWordDetails"Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="ToolFetchWordDetails.aspx.cs" Inherits="ToolFetchWordDetails" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <p></p>
    <table>
        <tr>
            <td>
                 <asp:Panel ID="Panel1" runat="server" Visible ="true" Height="420px" Width="250px">
                     <asp:TextBox ID="tbWords" runat="server" Height="390px" TextMode="MultiLine" Width="250px"></asp:TextBox>
                        <br />
                     <asp:Button ID="btnToList" runat="server" Text="ToList" OnClick="btnToList_Click"/>
                 </asp:Panel>
            </td>
            <td >
                 <asp:Panel ID="Panel2" runat="server" Visible ="false" Height="420px" Width="250px">
                     <asp:ListBox ID="lbWordList" runat="server" Height="390px"  Width="250px"  AutoPostBack="True" OnSelectedIndexChanged="lbWordList_SelectedIndexChanged"></asp:ListBox>
                         <br />
                     <asp:Button ID="btnBack" runat="server" Text="Back" OnClick="btnBack_Click"/>
                     <asp:Button ID="btnFetch" runat="server" Text="Fetch" OnClick="btnFetch_Click"/>
                 </asp:Panel>
            </td>
            <td >
                 <asp:Panel ID="Panel3" runat="server" Visible ="false" Height="420px" Width="250px">
                     <asp:Panel ID="Panel4" runat="server" Height="390px" Width="250px">
                         <asp:Label ID="lblWordTitle" runat="server" Text="Title"></asp:Label>
                         <br />
                         <asp:TextBox ID="txtWordBody" runat="server" Text="" Height="162px" TextMode="MultiLine" Width="250px"></asp:TextBox>
                         <asp:Label ID="lblParseResult" runat="server" Text=""></asp:Label>
                     </asp:Panel>
                     <asp:Button ID="btnInsertDB" runat="server" Text="InsertDB" OnClick="btnInsertDB_Click" />
                 </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
