import msql

class user:
    def __init__(self, id, client):
        msql.sql()
        self.id = id
        self.username = msql.sql().exeGetValues("SELECT benutzername FROM login WHERE id="+str(id), "benutzername")
        self.email = msql.sql().exeGetValues("SELECT email FROM login WHERE id="+str(id), "email")
        self.admin = msql.sql().exeGetValues("SELECT admin FROM login WHERE id="+str(id), "admin")
        self.client = client

