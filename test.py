import pyodbc

conn = pyodbc.connect(
    "DRIVER={SQL Server};"
    "SERVER=.\\SQLEXPRESS;"
    "DATABASE=EMS;"
    "Trusted_Connection=yes;"
    "TrustServerCertificate=yes;"
)

cursor = conn.cursor()

cursor.execute("SELECT COUNT(*) FROM Employee")

print(cursor.fetchone()[0])

conn.close()