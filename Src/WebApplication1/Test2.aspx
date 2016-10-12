<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test2.aspx.cs" Inherits="WebApplication1.Test2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/jQuery/jquery-1.11.3.js" />
            </Scripts>
        </asp:ScriptManager>
        <div>
            <asp:Panel runat="server" ID="pnlCal" />
        </div>

        <asp:UpdatePanel runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Button runat="server" ID="btnSubmit" Text="Submit" OnClick="Unnamed_Click" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
