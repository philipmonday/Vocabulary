<%@ Page Title="WordTesting" Language="C#"  Debug="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="WordTesting.aspx.cs" Inherits="WordTesting" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
     <h2><%: Title %></h2>
     <p>
        <asp:Label ID="lblWordBody" runat="server" Font-Bold="True" Font-Size="XX-Large" ForeColor="#0066FF" Text="Label"></asp:Label>
        <br />
        <asp:Label ID="lblCorrectOptionIndex" runat="server" Font-Size="Small" ForeColor="#0066FF" Text="Label" Visible ="false"></asp:Label>
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
     </p>
     <div>
         <asp:Table ID="InspectingTable" BorderStyle ="Solid" BorderWidth="5" runat="server" visible ="true" ></asp:Table>
     </div>
 
     <div>
         <asp:Table ID="HolderTable" BorderStyle ="Solid" BorderWidth="5" runat="server" visible ="true" ></asp:Table>
    </div>
</asp:Content>