using Ravlyk.SAE5.WinForms.Properties;
using System.Data.SQLite;
using System.IO;

public static class DatabaseHelper
{
    private static string connectionString = "Data Source=" + Settings.Default.UserPalettesLocation + "\\Files\\LibraryManagementSystem.db; Version=3;";

    public static void InitializeDatabase()
    {
        if (!File.Exists(Settings.Default.UserPalettesLocation + "\\Files\\LibraryManagementSystem.db"))
        {
            SQLiteConnection.CreateFile(Settings.Default.UserPalettesLocation + "\\Files\\LibraryManagementSystem.db");

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createProjectTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Projects (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ProjectImage BLOB,
                        ProjectName TEXT,
                        SizeHeight INTEGER,
                        SizeWidth INTEGER,
                        FormStones INTEGER,
                        NumberColors INTERGER,
                        StonesOrderedDate TEXT,
                        StonesOrderedAt TEXT,
                        StonesReceivedDate TEXT,
                        ProjectStartDate TEXT,
                        ProjectEndDate TEXT
                        
                        
                        
                        
                        
                );";


               /*string createSettingsTableQuery = @"
                    CREATE TABLE IF NOT EXISTS UserSettings (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        OwnSymbols NUMERIC NOT NULL DEFAULT 0,
                        OwnSymbolsColor NUMERIC NOT NULL DEFAULT 0
                        
                    );";

                string createPDFSettingsTableQuery = @"
                    CREATE TABLE IF NOT EXISTS PDFSettings (
                        Company TEXT,
                        Owner TEXT,
                        OwnerPassword TEXT,
                        PDFPassword TEXT,
                        PictureName TEXT,
                        PermitPrint NUMERIC NOT NULL DEFAULT 0,
                        PermitModifyDocument NUMERIC NOT NULL DEFAULT 0,
                        PermitAssembleDocument NUMERIC NOT NULL DEFAULT 0,
                        PermitExtractContent NUMERIC NOT NULL DEFAULT 0,
                        PermitAccessibilityExtractContent NUMERIC NOT NULL DEFAULT 0,
                        PermitAnnotations NUMERIC NOT NULL DEFAULT 0,
                        PermitFormsFill NUMERIC NOT NULL DEFAULT 0,
                        PermitFullQualityPrint NUMERIC NOT NULL DEFAULT 0,
                        PrintDeckblatt NUMERIC NOT NULL DEFAULT 0,
                        PrintEtiketten NUMERIC NOT NULL DEFAULT 0,
                        PrintScheme NUMERIC NOT NULL DEFAULT 0,
                        EtiUmrandung NUMERIC NOT NULL DEFAULT 0,
                        Mystery NUMERIC NOT NULL DEFAULT 0,
                        TicTacEti NUMERIC NOT NULL DEFAULT 0,
                        RundEti NUMERIC NOT NULL DEFAULT 0,
                        SymFull NUMERIC NOT NULL DEFAULT 0,
                        SymNormal NUMERIC NOT NULL DEFAULT 0,
                        LegendLeft NUMERIC NOT NULL DEFAULT 0,
                        WithoutDMC NUMERIC NOT NULL DEFAULT 0,
                        Logo TEXT,
                        MysteryPic TEXT
                    
                );";

                string createLicenceSettingsTableQuery = @"
                    CREATE TABLE IF NOT EXISTS LicenceSettings (
                        isLizenzValid NUMERIC NOT NULL DEFAULT 0,
                        isLizenzDemo NUMERIC NOT NULL DEFAULT 0,
                        isLizenzCommerc NUMERIC NOT NULL DEFAULT 0,
                        FT1 NUMERIC NOT NULL DEFAULT 0,
                        FT2 NUMERIC NOT NULL DEFAULT 0,
                        FT3 NUMERIC NOT NULL DEFAULT 0,
                        FT4 NUMERIC NOT NULL DEFAULT 0,
                        FT5 NUMERIC NOT NULL DEFAULT 0,
                        FT6 NUMERIC NOT NULL DEFAULT 0,
                        FT7 NUMERIC NOT NULL DEFAULT 0,
                        FT8 NUMERIC NOT NULL DEFAULT 0,
                        FT9 NUMERIC NOT NULL DEFAULT 0,
                        FT10 NUMERIC NOT NULL DEFAULT 0,

                        ForeName TEXT,
                        SureName TEXT,
                        Email TEXT

                );";
                */
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = createProjectTableQuery;
                    command.ExecuteNonQuery();

                    /*command.CommandText = createSettingsTableQuery;
                    command.ExecuteNonQuery();

                    command.CommandText = createPDFSettingsTableQuery;
                    command.ExecuteNonQuery();

                    command.CommandText = createLicenceSettingsTableQuery;
                    command.ExecuteNonQuery();
                    */
                }
            }
        }
    }
}