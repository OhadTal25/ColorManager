<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ManageColors.aspx.vb" Inherits="WebApplication1.ManageColors" %>
<link rel="stylesheet" type="text/css" href="StyleSheet1.css" />

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ניהול צבעים</title>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/smoothness/jquery-ui.css">
</head>
<body>
    <form id="form1" runat="server">
        <h1>טבלת צבעים</h1>
        <asp:GridView ID="ColorsGridView" runat="server" AutoGenerateColumns="False" CssClass="grid"
            OnRowCommand="ColorsGridView_RowCommand" OnRowDeleting="ColorsGridView_RowDeleting"
            OnRowEditing="ColorsGridView_RowEditing" ClientIDMode="Static">
            <Columns>
                <asp:BoundField DataField="ColorName" HeaderText="שם הצבע" />
                <asp:TemplateField HeaderText="צבע">
                    <ItemTemplate>
                        <div style='<%# "background-color:" + Eval("ColorValue") + "; color:" + GetTextColor(Eval("ColorValue").ToString()) + "; text-align: center; padding: 8px; border-radius: 5px;" %>'>
                            <%# Eval("ColorValue") %>
                        </div>

                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Price" HeaderText="מחיר" />
                <asp:BoundField DataField="DisplayOrder" HeaderText="סדר הצגה" />
                <asp:CheckBoxField DataField="InStock" HeaderText="במלאי" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button ID="btnEdit" runat="server" Text="ערוך" CommandName="Edit" CommandArgument='<%# Eval("ID") %>' CssClass="btn" />
                        <asp:Button ID="btnDelete" runat="server" Text="מחק" CommandName="Delete" CommandArgument='<%# Eval("ID") %>' CssClass="btn" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <div class="count">מספר רשומות: <asp:Literal ID="recordCountLiteral" runat="server"></asp:Literal></div>

        <h2>הוסף / עדכן צבע</h2>
        <label for="txtColorName">שם הצבע:</label>
        <asp:TextBox ID="txtColorName" runat="server"></asp:TextBox>
    
        <label for="txtPrice">מחיר:</label>
        <asp:TextBox ID="txtPrice" runat="server" TextMode="Number"></asp:TextBox>
    
        <label for="txtDisplayOrder">סדר הצגה:</label>
        <asp:TextBox ID="txtDisplayOrder" runat="server" TextMode="Number"></asp:TextBox>
    
        <label for="chkInStock">במלאי:</label>
        <asp:CheckBox ID="chkInStock" runat="server" /><br />
    
        <asp:Button ID="btnAddUpdate" runat="server" Text="הוסף / עדכן" CssClass="button-primary" OnClick="btnAddUpdate_Click" />

    </form>



    <script>
        $(document).ready(function () {
            // Enable sorting on the GridView
            $("#ColorsGridView tbody").sortable({
                update: function (event, ui) {
                    var order = [];
                    // Extract the IDs of the rows in their new order
                    $("#ColorsGridView tbody tr").each(function () {
                        order.push($(this).data("id"));
                    });

                    // Send the new order to the server
                    $.ajax({
                        type: "POST",
                        url: "ManageColors.aspx/UpdateOrder",
                        data: JSON.stringify({ order: order }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function () {
                            alert("סדר הצגה עודכן בהצלחה!");
                        },
                    });
                }
            }).disableSelection();
        });
    </script>


</body>
</html>
