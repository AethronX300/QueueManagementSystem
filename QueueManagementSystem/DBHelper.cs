using System;
using System.Data;
using System.Data.SQLite;
using System.Configuration;
using System.Web;

namespace QueueManagementSystem
{
    /// <summary>
    /// ADO.NET Database Helper Class using SQLite
    /// Demonstrates: ADO.NET, Managed Providers, Data Binding, Datasets
    /// The database file (QueueManagement.db) is stored in App_Data folder
    /// and can be copied to transfer data between devices.
    /// </summary>
    public static class DBHelper
    {
        /// <summary>
        /// Returns the SQLite connection string, resolving |DataDirectory| to App_Data
        /// </summary>
        private static string GetConnectionString()
        {
            string connStr = ConfigurationManager.ConnectionStrings["QueueDB"].ConnectionString;
            if (connStr.Contains("|DataDirectory|"))
            {
                string dataDir = HttpContext.Current != null
                    ? HttpContext.Current.Server.MapPath("~/App_Data/")
                    : AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString() ?? "";
                connStr = connStr.Replace("|DataDirectory|", dataDir);
            }
            return connStr;
        }

        /// <summary>
        /// Returns a new SQLiteConnection object (ADO.NET Managed Provider)
        /// </summary>
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(GetConnectionString());
        }

        /// <summary>
        /// Executes INSERT, UPDATE, DELETE queries using parameterized commands
        /// Returns the number of rows affected
        /// </summary>
        public static int ExecuteNonQuery(string query, SQLiteParameter[] parameters = null)
        {
            int result = 0;
            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
            }
            return result;
        }

        /// <summary>
        /// Executes SELECT queries and returns results as DataTable (Dataset concept)
        /// </summary>
        public static DataTable ExecuteReader(string query, SQLiteParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// Executes a query and returns a single value (first column of first row)
        /// </summary>
        public static object ExecuteScalar(string query, SQLiteParameter[] parameters = null)
        {
            object result = null;
            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    result = cmd.ExecuteScalar();
                }
            }
            return result;
        }

        /// <summary>
        /// Executes an INSERT and returns the last inserted row ID on the same connection
        /// </summary>
        public static int ExecuteInsertAndGetId(string query, SQLiteParameter[] parameters = null)
        {
            int id = 0;
            using (SQLiteConnection conn = GetConnection())
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    cmd.ExecuteNonQuery();
                }
                using (SQLiteCommand idCmd = new SQLiteCommand("SELECT last_insert_rowid()", conn))
                {
                    id = Convert.ToInt32(idCmd.ExecuteScalar());
                }
            }
            return id;
        }

        /// <summary>
        /// Generates the next token number for a business on the current day
        /// Format: PREFIX + 3-digit number (e.g., R001, C002, B003)
        /// </summary>
        public static string GetNextTokenNumber(int businessId, string prefix)
        {
            string query = @"SELECT COUNT(*) + 1 FROM Tokens 
                           WHERE BusinessId = @BusinessId 
                           AND date(CreatedDate) = date('now','localtime')";
            SQLiteParameter[] parameters = { new SQLiteParameter("@BusinessId", businessId) };
            object result = ExecuteScalar(query, parameters);
            int count = Convert.ToInt32(result);
            return prefix + count.ToString("D3");
        }

        /// <summary>
        /// Generates the next token for a specific TABLE in a restaurant.
        /// Format: T{tableNumber}-{2-digit-count} e.g. T2-05 means 5th customer at Table 2 today
        /// </summary>
        public static string GetNextTableTokenNumber(int businessId, string tableNumber)
        {
            string query = @"SELECT COUNT(*) + 1 FROM Tokens
                            WHERE BusinessId = @BusinessId
                            AND TableNumber = @TableNumber
                            AND date(CreatedDate) = date('now','localtime')";
            SQLiteParameter[] parameters = {
                new SQLiteParameter("@BusinessId", businessId),
                new SQLiteParameter("@TableNumber", tableNumber)
            };
            object result = ExecuteScalar(query, parameters);
            int count = Convert.ToInt32(result);
            return "T" + tableNumber + "-" + count.ToString("D2");
        }

        /// <summary>
        /// Initializes the SQLite database - creates tables and seed data
        /// Called from Global.asax Application_Start
        /// </summary>
        public static void InitializeDatabase()
        {
            string appDataPath = HttpContext.Current.Server.MapPath("~/App_Data/");
            if (!System.IO.Directory.Exists(appDataPath))
                System.IO.Directory.CreateDirectory(appDataPath);

            string dbPath = System.IO.Path.Combine(appDataPath, "QueueManagement.db");
            if (!System.IO.File.Exists(dbPath))
                SQLiteConnection.CreateFile(dbPath);

            using (SQLiteConnection conn = GetConnection())
            {
                conn.Open();

                // Create all tables
                string createTables = @"
                    CREATE TABLE IF NOT EXISTS BusinessTypes (
                        BusinessTypeId INTEGER PRIMARY KEY AUTOINCREMENT,
                        TypeName TEXT NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS Businesses (
                        BusinessId INTEGER PRIMARY KEY AUTOINCREMENT,
                        BusinessTypeId INTEGER NOT NULL,
                        BusinessName TEXT NOT NULL,
                        Address TEXT,
                        ContactNumber TEXT,
                        CreatedDate TEXT DEFAULT (datetime('now','localtime')),
                        FOREIGN KEY (BusinessTypeId) REFERENCES BusinessTypes(BusinessTypeId)
                    );
                    CREATE TABLE IF NOT EXISTS Users (
                        UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                        BusinessId INTEGER NOT NULL,
                        Username TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL,
                        FullName TEXT NOT NULL,
                        Role TEXT DEFAULT 'Admin',
                        CreatedDate TEXT DEFAULT (datetime('now','localtime')),
                        FOREIGN KEY (BusinessId) REFERENCES Businesses(BusinessId)
                    );
                    CREATE TABLE IF NOT EXISTS ServiceCounters (
                        CounterId INTEGER PRIMARY KEY AUTOINCREMENT,
                        BusinessId INTEGER NOT NULL,
                        CounterName TEXT NOT NULL,
                        IsActive INTEGER DEFAULT 1,
                        FOREIGN KEY (BusinessId) REFERENCES Businesses(BusinessId)
                    );
                    CREATE TABLE IF NOT EXISTS Tokens (
                        TokenId INTEGER PRIMARY KEY AUTOINCREMENT,
                        BusinessId INTEGER NOT NULL,
                        TokenNumber TEXT NOT NULL,
                        CustomerName TEXT NOT NULL,
                        ContactNumber TEXT,
                        ServiceType TEXT,
                        CounterId INTEGER,
                        TableNumber TEXT,
                        CurrentStatus TEXT NOT NULL,
                        PaymentStatus TEXT DEFAULT 'Pending',
                        CreatedDate TEXT DEFAULT (datetime('now','localtime')),
                        CompletedDate TEXT,
                        FOREIGN KEY (BusinessId) REFERENCES Businesses(BusinessId),
                        FOREIGN KEY (CounterId) REFERENCES ServiceCounters(CounterId)
                    );
                    CREATE TABLE IF NOT EXISTS TokenStatusLog (
                        LogId INTEGER PRIMARY KEY AUTOINCREMENT,
                        TokenId INTEGER NOT NULL,
                        Status TEXT NOT NULL,
                        ChangedBy INTEGER NOT NULL,
                        ChangedDate TEXT DEFAULT (datetime('now','localtime')),
                        FOREIGN KEY (TokenId) REFERENCES Tokens(TokenId),
                        FOREIGN KEY (ChangedBy) REFERENCES Users(UserId)
                    );
                ";
                using (SQLiteCommand cmd = new SQLiteCommand(createTables, conn))
                    cmd.ExecuteNonQuery();

                // Seed business types if empty
                using (SQLiteCommand checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM BusinessTypes", conn))
                {
                    if ((long)checkCmd.ExecuteScalar() == 0)
                    {
                        string seed = @"
                            INSERT INTO BusinessTypes (TypeName) VALUES ('Restaurant');
                            INSERT INTO BusinessTypes (TypeName) VALUES ('Clinic');
                            INSERT INTO BusinessTypes (TypeName) VALUES ('Bank');
                        ";
                        using (SQLiteCommand seedCmd = new SQLiteCommand(seed, conn))
                            seedCmd.ExecuteNonQuery();
                    }
                }

                // Always ensure demo businesses exist (INSERT OR IGNORE = safe to re-run)
                using (SQLiteCommand c = new SQLiteCommand(@"
                    INSERT OR IGNORE INTO Businesses (BusinessId, BusinessTypeId, BusinessName, Address, ContactNumber)
                    VALUES (1, 1, 'Spice Garden Restaurant', '12 MG Road, Bangalore', '9876543210');
                    INSERT OR IGNORE INTO Businesses (BusinessId, BusinessTypeId, BusinessName, Address, ContactNumber)
                    VALUES (2, 2, 'Apollo Clinic', '45 Park Street, Chennai', '9123456780');
                    INSERT OR IGNORE INTO Businesses (BusinessId, BusinessTypeId, BusinessName, Address, ContactNumber)
                    VALUES (3, 3, 'State Bank of India - Main Branch', '1 Bank Road, Mumbai', '9988776655');
                ", conn)) c.ExecuteNonQuery();

                // Always ensure login accounts exist
                using (SQLiteCommand c = new SQLiteCommand(@"
                    INSERT OR IGNORE INTO Users (UserId, BusinessId, Username, Password, FullName, Role)
                    VALUES (1, 1, 'restaurant_admin', 'admin123', 'Ravi Kumar', 'Admin');
                    INSERT OR IGNORE INTO Users (UserId, BusinessId, Username, Password, FullName, Role)
                    VALUES (2, 2, 'clinic_admin', 'admin123', 'Dr. Priya Sharma', 'Admin');
                    INSERT OR IGNORE INTO Users (UserId, BusinessId, Username, Password, FullName, Role)
                    VALUES (3, 3, 'bank_admin', 'admin123', 'Suresh Menon', 'Admin');
                    INSERT OR IGNORE INTO Users (UserId, BusinessId, Username, Password, FullName, Role)
                    VALUES (4, 1, 'hi', 'bye', 'Quick Login (Restaurant)', 'Admin');
                    INSERT OR IGNORE INTO Users (UserId, BusinessId, Username, Password, FullName, Role)
                    VALUES (5, 2, 'hi_clinic', 'bye', 'Quick Login (Clinic)', 'Admin');
                    INSERT OR IGNORE INTO Users (UserId, BusinessId, Username, Password, FullName, Role)
                    VALUES (6, 3, 'hi_bank', 'bye', 'Quick Login (Bank)', 'Admin');
                ", conn)) c.ExecuteNonQuery();

                // Always ensure bank service counters exist
                using (SQLiteCommand c = new SQLiteCommand(@"
                    INSERT OR IGNORE INTO ServiceCounters (CounterId, BusinessId, CounterName, IsActive) VALUES (1, 3, 'Counter 1', 1);
                    INSERT OR IGNORE INTO ServiceCounters (CounterId, BusinessId, CounterName, IsActive) VALUES (2, 3, 'Counter 2', 1);
                    INSERT OR IGNORE INTO ServiceCounters (CounterId, BusinessId, CounterName, IsActive) VALUES (3, 3, 'Counter 3', 1);
                ", conn)) c.ExecuteNonQuery();

                // Seed demo tokens ONLY if today has fewer than 30 tokens
                // This ensures fresh data with today's date is always visible on the dashboards
                using (SQLiteCommand checkTokens = new SQLiteCommand(
                    "SELECT COUNT(*) FROM Tokens WHERE date(CreatedDate) = date('now','localtime')", conn))
                {
                    long tokenCount = (long)checkTokens.ExecuteScalar();
                    if (tokenCount < 30)
                    {
                        // Restaurant - 10 tokens
                        using (SQLiteCommand c = new SQLiteCommand(@"
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, TableNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (1, 'T5-01',  'Amit Sharma',  '9812345678', '5',  'Dine-In', 'Order Placed',   'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, TableNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (1, 'T2-01',  'Priya Singh',  '9876501234', '2',  'Dine-In', 'Preparing',      'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, TableNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (1, 'T8-01',  'Rahul Verma',  '9988776611', '8',  'Dine-In', 'Ready to Serve', 'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, TableNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (1, 'T1-01',  'Sneha Patil',  '9123456099', '1',  'Dine-In', 'Served',         'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, TableNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (1, 'T3-01',  'Karthik Nair', '9456781234', '3',  'Dine-In', 'Served',         'Paid',    datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, TableNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (1, 'T6-01',  'Neha Gupta',   '9234567890', '6',  'Dine-In', 'Order Placed',   'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, TableNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (1, 'T4-01',  'Vikram Rao',   '9870001234', '4',  'Dine-In', 'Preparing',      'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, TableNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (1, 'T9-01',  'Deepak Mehta', '9445566778', '9',  'Dine-In', 'Order Placed',   'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, TableNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (1, 'T10-01', 'Pooja Reddy',  '9001122334', '10', 'Dine-In', 'Order Placed',   'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, TableNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (1, 'T2-02',  'Rohan Mehta',  '9800011223', '2',  'Dine-In', 'Order Placed',   'Pending', datetime('now','localtime'));
                        ", conn)) c.ExecuteNonQuery();

                        // Clinic - 10 tokens
                        using (SQLiteCommand c = new SQLiteCommand(@"
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (2, 'C001', 'Ramesh Iyer',     '9876543000', 'General Consultation', 'Completed',       'Paid',    datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (2, 'C002', 'Lakshmi Devi',    '9812340099', 'General Consultation', 'In Consultation',  'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (2, 'C003', 'Mohan Pillai',    '9911002233', 'Pediatrics',           'Waiting',          'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (2, 'C004', 'Sunita Roy',      '9123000456', 'Dermatology',          'Waiting',          'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (2, 'C005', 'Kavitha Menon',   '9988001122', 'Gynecology',           'Waiting',          'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (2, 'C006', 'Sanjay Patel',    '9000123456', 'Orthopedics',          'Waiting',          'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (2, 'C007', 'Geetha Krishnan', '9876100234', 'General Consultation', 'Completed',        'Paid',    datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (2, 'C008', 'Dinesh Kumar',    '9123987654', 'Psychiatry',           'Waiting',          'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (2, 'C009', 'Meena Shetty',    '9912334455', 'Endocrinology',        'Waiting',          'Pending', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (2, 'C010', 'Arjun Nambiar',   '9567891234', 'General Consultation', 'Waiting',          'Pending', datetime('now','localtime'));
                        ", conn)) c.ExecuteNonQuery();

                        // Bank - 10 tokens
                        using (SQLiteCommand c = new SQLiteCommand(@"
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CounterId, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (3, 'B001', 'Anil Kapoor',    '9812300001', 'Deposit',         1, 'At Counter', 'NA', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CounterId, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (3, 'B002', 'Ritu Sharma',    '9900112233', 'Withdrawal',      1, 'Waiting',    'NA', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CounterId, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (3, 'B003', 'Suresh Babu',    '9123456001', 'Account Opening', 2, 'Waiting',    'NA', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CounterId, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (3, 'B004', 'Manju Latha',    '9878001200', 'Loan Inquiry',    2, 'At Counter', 'NA', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CounterId, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (3, 'B005', 'Pradeep Nair',   '9001234509', 'Withdrawal',      3, 'Waiting',    'NA', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CounterId, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (3, 'B006', 'Swati Joshi',    '9812399001', 'Deposit',         1, 'Waiting',    'NA', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CounterId, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (3, 'B007', 'Naresh Gupta',   '9988001230', 'Other',           3, 'Waiting',    'NA', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CounterId, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (3, 'B008', 'Kavya Rao',      '9001120011', 'Account Opening', 2, 'Completed',  'NA', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CounterId, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (3, 'B009', 'Harish Menon',   '9876500001', 'Loan Inquiry',    3, 'Completed',  'NA', datetime('now','localtime'));
                            INSERT INTO Tokens (BusinessId, TokenNumber, CustomerName, ContactNumber, ServiceType, CounterId, CurrentStatus, PaymentStatus, CreatedDate)
                            VALUES (3, 'B010', 'Divya Krishnan', '9123000001', 'Withdrawal',      1, 'Completed',  'NA', datetime('now','localtime'));
                        ", conn)) c.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
