Imports System.Net
Imports Newtonsoft.Json

Class MainWindow

    Private Sub search_Button_Click(sender As Object, e As RoutedEventArgs)
        ' Change this to your Search Key
        Dim searchKey As String = "SEARCH KEY GOES HERE"

        If searchKey = "SEARCH KEY GOES HERE" Then
            MessageBox.Show("You need to set the searchKey variable to your Search Key (available from the admin interface online)", "Warning", MessageBoxButton.OK, MessageBoxImage.Information)
            Exit Sub
        End If

        ' Any search can include an optional identifier, this your own value and can be set to whatever you like for tracking purposes
        Dim identifier As String = "VBNetSharpExample"

        Dim restPattern As String = "https://ws.postcoder.com/pcw/{0}/Address/UK/{1}?identifier={2}"

        Dim searchTest As String = search_TextBox.Text

        Dim restURL As String = String.Format(restPattern, searchKey, searchTest, identifier)

        ' The defaults for this will return JSON, if you prefer XML then alter the accept header.
        Dim web As New WebClient()

        ' This will contain the main return from the service
        Dim json As String = ""

        Try
            ' This is the call to the online service
            json = web.DownloadString(restURL)
        Catch webEx As WebException
            ' Something has gone wrong, 403 a parameter is incorrect. http://ws.postcoder.com/pcw/YOUR_SEARCH_KEY/status will usually show what's up.
            output_TextBox.Text = webEx.ToString()
            Return
        End Try

        ' Maps the JSON response to the .NET object (defined below)
        Dim addresses As Address() = JsonConvert.DeserializeObject(Of Address())(json)

        ' Load the forms combo box
        addresses_ComboBox.Items.Clear()
        If addresses.Length > 0 Then
            For Each address As Address In addresses
                addresses_ComboBox.Items.Add(address)
            Next

            addresses_ComboBox.SelectedIndex = 0
        End If

        ' Show the Raw output in an indented format.
        output_TextBox.Text = JsonToIndentedString(json)
    End Sub

    Public Function JsonToIndentedString(json As String) As String
        Dim parsedJson = JsonConvert.DeserializeObject(json)
        Return JsonConvert.SerializeObject(parsedJson, Formatting.Indented)
    End Function

    Private Sub addresses_ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If addresses_ComboBox.Items.Count <= 0 Then
            Return
        End If

        If addresses_ComboBox.SelectedIndex < 0 Then
            addresses_ComboBox.SelectedIndex = 0
        End If

        Dim address As Address = DirectCast(addresses_ComboBox.SelectedItem, Address)

        organisation_TextBox.Text = address.organisation
        premise_TextBox.Text = address.premise
        dependentStreet_TextBox.Text = address.dependentstreet
        street_TextBox.Text = address.street
        doubleDependentLocality_TextBox.Text = address.doubledependentlocality
        locality_TextBox.Text = address.dependentlocality
        town_TextBox.Text = address.posttown
        county_TextBox.Text = address.county
        postcode_TextBox.Text = address.postcode
    End Sub
End Class

Public Class Address

    Public organisation As String
    Public premise As String
    Public dependentstreet As String
    Public street As String
    Public doubledependentlocality As String
    Public dependentlocality As String
    Public posttown As String
    Public county As String
    Public postcode As String

    Public summaryline As String

    Public Overrides Function ToString() As String
        Return summaryline
    End Function

End Class
