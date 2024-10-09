namespace Metadata.Dbs.Postgres

module DQL =

    open System.Text.Json
    open System.Data
    open System.Collections.Generic
    open Npgsql

    let getReader (connString: string) (query: string) (parameters: (string * obj) list) : Common.DbDataReader =
        let conn = new NpgsqlConnection(connString)
        conn.Open()

        let cmd = new NpgsqlCommand(query, conn)
        parameters |> List.iter (fun (name, value) -> cmd.Parameters.AddWithValue(name, value) |> ignore)

        let reader = cmd.ExecuteReader()
        reader

    let convertReaderToDataTable (reader: NpgsqlDataReader): DataTable =
        let dataTable = new DataTable()
        dataTable.Load(reader)
        reader.Close()
        dataTable

    let convertReaderToCollection (reader: NpgsqlDataReader) : List<Dictionary<string, obj>> =
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

    let convertReaderToJson (reader: NpgsqlDataReader) : string =
        JsonSerializer.Serialize(convertReaderToCollection reader)