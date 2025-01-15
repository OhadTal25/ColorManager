Imports System.Data.SqlClient
Imports System.Web.Services

Public Class ManageColors
    Inherits System.Web.UI.Page

    Private connectionString As String = "Server=localhost;Database=ColorsDB;Trusted_Connection=True;"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        CreateDatabaseAndTable()
        If Not IsPostBack Then
            LoadColors()
        End If
    End Sub

    Private Sub LoadColors()
        Using conn As New SqlConnection(connectionString)
            Dim query As String = "SELECT * FROM Colors ORDER BY DisplayOrder"
            Dim cmd As New SqlCommand(query, conn)
            Dim adapter As New SqlDataAdapter(cmd)
            Dim table As New DataTable()
            adapter.Fill(table)

            ColorsGridView.DataSource = table
            ColorsGridView.DataBind()

            ' Update record count
            recordCountLiteral.Text = table.Rows.Count.ToString()
        End Using
    End Sub
    Protected Sub btnAddUpdate_Click1(sender As Object, e As EventArgs)
        Response.Write("<script>alert('האירוע הופעל בהצלחה');</script>")

    End Sub

    Protected Sub btnAddUpdate_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtColorName.Text) Then
            Response.Write("<script>alert('Please enter a valid color name.');</script>")
            Return
        End If


        Dim hexValue As String = GenerateHexFromName(txtColorName.Text.Trim())

        Dim price As Decimal
        If Not Decimal.TryParse(txtPrice.Text, price) Then
            Response.Write("<script>alert('Please enter a valid price.');</script>")
            Return
        End If

        Dim displayOrder As Integer
        If Not Integer.TryParse(txtDisplayOrder.Text, displayOrder) Then
            Response.Write("<script>alert('Please enter a valid display order.');</script>")
            Return
        End If

        Using conn As New SqlConnection(connectionString)
            conn.Open()
            Dim query As String
            If ViewState("ColorID") Is Nothing Then
                query = "INSERT INTO Colors (ColorName, ColorValue, Price, DisplayOrder, InStock) VALUES (@ColorName, @ColorValue, @Price, @DisplayOrder, @InStock)"
            Else
                query = "UPDATE Colors SET ColorName = @ColorName, ColorValue = @ColorValue, Price = @Price, DisplayOrder = @DisplayOrder, InStock = @InStock WHERE ID = @ID"
            End If

            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@ColorName", txtColorName.Text.Trim())
                cmd.Parameters.AddWithValue("@ColorValue", hexValue) ' Auto-generated HEX value
                cmd.Parameters.AddWithValue("@Price", price)
                cmd.Parameters.AddWithValue("@DisplayOrder", displayOrder)
                cmd.Parameters.AddWithValue("@InStock", chkInStock.Checked)

                If ViewState("ColorID") IsNot Nothing Then
                    cmd.Parameters.AddWithValue("@ID", ViewState("ColorID"))
                End If

                cmd.ExecuteNonQuery()
            End Using
        End Using

        ClearForm()
        LoadColors()
    End Sub

    Public Function GetTextColor(hexColor As String) As String
        If hexColor = "#FFFFFF" Then
            Return "#000000" ' Black for white background
        Else
            Return "#FFFFFF" ' White for other backgrounds
        End If
    End Function

    Private Function GenerateHexFromName(colorName As String) As String
        ' Dictionary of known colors
        Dim colorMap As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
            {"כחול", "#0000FF"},
            {"ירוק", "#00FF00"},
            {"אדום", "#FF0000"},
            {"צהוב", "#FFFF00"},
            {"שחור", "#000000"},
            {"לבן", "#FFFFFF"},
            {"סגול", "#800080"},
            {"תכלת", "#ADD8E6"},
            {"חום", "#A52A2A"},
            {"ורוד", "#FFC0CB"},
            {"כתום", "#FFA500"}
        }

        ' Try to get the color from the map
        If colorMap.ContainsKey(colorName.Trim()) Then
            Return colorMap(colorName.Trim())
        End If

        ' Default color if not found
        Return "#808080" ' Gray as default
    End Function

    Private Sub ClearForm()
        txtColorName.Text = ""
        txtPrice.Text = ""
        txtDisplayOrder.Text = ""
        chkInStock.Checked = False
        ViewState("ColorID") = Nothing
    End Sub

    Protected Sub ColorsGridView_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If e.CommandName = "Delete" Then
            ' Handle the delete action
            Dim colorID As Integer = Convert.ToInt32(e.CommandArgument)

            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim query As String = "DELETE FROM Colors WHERE ID = @ID"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@ID", colorID)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' Reload the grid
            LoadColors()
        ElseIf e.CommandName = "Edit" Then
            ' Handle the edit action
            Dim colorID As Integer = Convert.ToInt32(e.CommandArgument)

            Using conn As New SqlConnection(connectionString)
                conn.Open()
                Dim query As String = "SELECT * FROM Colors WHERE ID = @ID"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@ID", colorID)
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            txtColorName.Text = reader("ColorName").ToString()
                            txtPrice.Text = reader("Price").ToString()
                            txtDisplayOrder.Text = reader("DisplayOrder").ToString()
                            chkInStock.Checked = Convert.ToBoolean(reader("InStock"))
                            ViewState("ColorID") = colorID
                        End If
                    End Using
                End Using
            End Using
        End If
    End Sub

    Protected Sub ColorsGridView_RowEditing(sender As Object, e As GridViewEditEventArgs)
        ' Suppress the default RowEditing behavior
        e.Cancel = True
    End Sub

    Protected Sub ColorsGridView_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        ' Suppress the default RowDeleting behavior
        e.Cancel = True
    End Sub
    <WebMethod()>
    Public Shared Sub UpdateOrder(order As List(Of Integer))
        Dim connectionString As String = "Server=localhost;Database=ColorsDB;Trusted_Connection=True;"
        Using conn As New SqlConnection(connectionString)
            conn.Open()
            For i As Integer = 0 To order.Count - 1
                Dim query As String = "UPDATE Colors SET DisplayOrder = @DisplayOrder WHERE ID = @ID"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@DisplayOrder", i + 1) ' Update order starting from 1
                    cmd.Parameters.AddWithValue("@ID", order(i))
                    cmd.ExecuteNonQuery()
                End Using
            Next
        End Using
    End Sub


    Private Sub CreateDatabaseAndTable()
        Dim masterConnectionString As String = "Server=localhost;Trusted_Connection=True;"

        Using conn As New SqlConnection(masterConnectionString)
            conn.Open()
            Dim createDatabaseQuery As String = "IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = 'ColorsDB') CREATE DATABASE ColorsDB;"
            Using cmd As New SqlCommand(createDatabaseQuery, conn)
                cmd.ExecuteNonQuery()
            End Using

            conn.ChangeDatabase("ColorsDB")

            Dim createTableQuery As String = "
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Colors')
            CREATE TABLE Colors (
                ID INT IDENTITY(1,1) PRIMARY KEY,
                ColorName NVARCHAR(50) NOT NULL,
                ColorValue NVARCHAR(10) NOT NULL,
                Price DECIMAL(10,2) NOT NULL,
                DisplayOrder INT NOT NULL,
                InStock BIT NOT NULL
            );"
            Using cmd As New SqlCommand(createTableQuery, conn)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
End Class
