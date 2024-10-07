namespace DbModel.Dbs.SQLite

module Query =

    open System.Text.Json
    open System.Data
    open System.Data.SQLite
    open System.Collections.Generic

    let getReader (connString: string) (query: string) (parameters: (string * obj) list) : SQLiteDataReader =
        let conn = new SQLiteConnection(connString)
        conn.Open()

        let cmd = new SQLiteCommand(query, conn)
        parameters |> List.iter (fun (name, value) -> cmd.Parameters.AddWithValue(name, value) |> ignore)

        let reader = cmd.ExecuteReader()
        reader

    let convertReaderToDataTable (reader: SQLiteDataReader) : DataTable =
        let dataTable = new DataTable()
        dataTable.Load(reader) 
        reader.Close() 
        dataTable

    let convertReaderToCollection (reader: SQLiteDataReader) : List<Dictionary<string, obj>> =
        let readRow (reader: IDataReader) =
            let dict = Dictionary<string, obj>()
            for i in 0 .. reader.FieldCount - 1 do
                dict.[reader.GetName(i)] <- if reader.IsDBNull(i) then null else reader.GetValue(i)
            dict

        let listOfRows = List<Dictionary<string, obj>>()
        while reader.Read() do
            listOfRows.Add(readRow reader)
        reader.Close() 
        listOfRows

    let convertReaderToJson (reader: SQLiteDataReader) : string =
        JsonSerializer.Serialize(convertReaderToCollection reader)
