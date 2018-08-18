#import MySQLdb
import hashlib
import pymysql as MySQLdb

#127.0.0.1
class sql:
    def __init__(self, host="127.0.0.1", user="dbuser", passwd="dbpw", db="db"):
        self.host = host
        self.user = user
        self.passwd = passwd
        self.db = db
        #print("[SQL] Success !")

    def connect(self):
        return MySQLdb.connect(self.host, self.user, self.passwd, self.db)

    def login(self, benutzername, passwd):
        db = self.connect()
        cur = db.cursor()
        cur.execute("SELECT id, benutzername, passwort, ban FROM login")
        hash_passwd = hashlib.md5(passwd.encode())
        #print(hash_passwd.hexdigest())
        for j in cur.fetchall():
            #print(j[1])
            if benutzername == j[1] and hash_passwd.hexdigest() == j[2]:
                if j[3] == 1:
                    return 2  # j[0]
                return 1#j[0]
        return 0

    def getID(self, benutzername, passwd):
        db = self.connect()
        cur = db.cursor()
        cur.execute("SELECT id, benutzername, passwort, ban FROM login")
        hash_passwd = hashlib.md5(passwd.encode())
        #print(hash_passwd.hexdigest())
        for j in cur.fetchall():
            #print(j[1])
            if benutzername == j[1] and hash_passwd.hexdigest() == j[2]:
                return j[0]
        return 0

    def ban(self, banID):
        db = self.connect()
        cur = db.cursor()
        cur.execute("UPDATE login SET ban=1 WHERE id="+str(banID))
        print("banned " + str(banID))
        print(cur.DataError())
        db.commit()

    def exeGetValues(self, command, selectName):
        db = self.connect()
        cur = db.cursor()
        cur.execute(command)
        retValue = 0
        for j in cur.fetchall():
            retValue = j[0]
        return retValue
"""
if __name__ == '__main__':
    id = 7
    #sql()
    sql().ban(7)
    #print(sql().exeGetValues("SELECT benutzername FROM login WHERE id="+str(id), "benutzername"))
"""
# .fetchone() listet alle daten einer zeile auf
# .fetchone()listet alle zeilen auf