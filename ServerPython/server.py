import socket
import threading
import time
import msql
import user


class chatServer(threading.Thread):
    def __init__(self, host="127.0.0.1", port=8217):
        self.host = host
        self.port = port
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.users = []
        self.sock.bind((self.host, self.port))

    def listen(self):
        self.sock.listen(5)
        #msql.sql()
        print("Waiting for connections on PORT "+str(self.port))
        while True:
            client, address = self.sock.accept()
            #client.settimeout(120)
            self.login(client, address)

    def login(self, client, address):
        try:
            data = client.recv(1024)
            sKey = "ServerKey"
            cdata = data.decode('utf-8')
            name = ""
            Pass = ""
            change = True
            if sKey in cdata and "N: " in cdata and "P: " in cdata:
                for j in range(len(sKey)+3, len(data.decode('utf-8'))):
                    if cdata[j] == "P" and cdata[j+1] == ":" and cdata[j+2] == " ":
                        j += 3
                        for h in range(j, len(data.decode('utf-8'))):
                            Pass += cdata[h]
                        change=False
                    if change:
                        name += cdata[j]
                    else:
                        break

                loginCheck = msql.sql().login(name, Pass)
                userID = msql.sql().getID(name, Pass)
                for use in self.users:
                    if use.id == userID:
                        print("[SERVER]" + use.username , "ist bereits eingeloggt")
                        client.send("d".encode('utf-8'))
                        client.close()
                        break
                if loginCheck == 1:
                    client.send("success".encode('utf-8'))
                    self.users.append(user.user(msql.sql().getID(name, Pass), client))
                    print("[SERVER] " + self.users[self.getClientID(client)].username + " hat sich eingeloggt")

                    self.sendData(client, address)
                    print("data sended")
                    self.loginMessage(client)
                    threading.Thread(target=self.listenToClient, args=(client, address)).start()
                    threading.Thread(target=self.onlineList).start()
                elif loginCheck == 2:
                    print("[SERVER] Loginversuch: " + name + " (gesperrt)")
                    client.send("b".encode('utf-8'))
                    client.close()
                else:
                    client.send(" ".encode('utf-8'))
                    print("[SERVER] Dieser Account Exsistiert nicht :" + name)
                    client.close()
            else:
                client.close()
                print("Wrong Packet...")
        except:
            client.close()

    def onlineList(self):
        while True:
            onlineListe = ""
            for k in range(0, len(self.users)):
                if k == len(self.users)-1:
                    onlineListe += self.users[k].username
                else:
                    onlineListe += self.users[k].username+","
            msg = onlineListe
            time.sleep(1)
            if onlineListe:
                self.sendMessageToAll(msg, 2)

    def getClientID(self, client):
        ID = 0
        for t in range(0, len(self.users)):
            if self.users[t].client == client:
                ID = t
                break
        return ID

    def getUserID(self, client):
        ID = 0
        for t in range(0, len(self.users)):
            if self.users[t].client == client:
                ID = self.users[t].id
                break
        return ID

    def listenToClient(self, client, address):
        while True:
            try:
                header = client.recv(4).decode('utf-8')  # Magic, small number to begin with.
                if header:
                    while ":" not in header:
                        header += client.recv(1).decode('utf-8')  # Keep looping, picking up two bytes each time
                    size_of_package, separator, message_fragment = header.partition(":")
                    packSize = ""
                    change = False
                    action = ""
                    for k in range(0, len(size_of_package)):
                        if size_of_package[k] == ",":
                            change = True
                        else:
                            if change:
                                #print(size_of_package[k] + " " + str(k))
                                action += size_of_package[k]
                            else:
                                packSize += size_of_package[k]

                    message = client.recv(int(packSize))
                    full_message = message_fragment + message.decode('utf-8')
                    #print(full_message)
                    if int(action) == 0: # Send Message to All !!
                        msg = (self.users[self.getClientID(client)].username + ": " + full_message)
                        self.sendMessageToAll(msg, 0)
                    elif int(action) == 1: # Kick
                        for user in self.users:
                            if user.username == full_message:
                                id=self.getClientID(client)
                                if self.users[id].admin >= 1:
                                    user.client.close()
                                    print(user.username+ " kicked by: "+ self.users[id].username)
                    elif int(action) == 2: # ban
                        for user in self.users:
                            if user.username == full_message:
                                id=self.getClientID(client)
                                if self.users[id].admin >= 3:
                                    #user.client.send((self.users[id].username).encode('utf-8'))
                                    msql.sql().ban(user.id)
                                    user.client.close()
                                    print(user.username + " banned by: " + self.users[id].username)
                else:
                    print("Client disconnected")
                    client.close()
                    #raise ('Client disconnected')
            except:
                print("Listen end...")
                client.close()
                break

    def refresh(self):
        #print("REFRESHHHHH")
        for k in range(0, len(self.users)):
            if "[closed]" in str(self.users[k].client):
                print("[SERVER] "+self.users[k].username+" disconnected")
                print(self.users[k].client)
                self.users.remove(self.users[k])
                break

    def loginMessage(self, client):
        try:
            msg = ("[SERVER] " + self.users[self.getClientID(client)].username + " hat sich eingeloggt !")
            self.sendMessageToAll(msg, 0)
        except:
            print("Client disconnected")
            client.close()
            #raise ('Client disconnected')

    def sendData(self, client, address):
        for k in self.users:
            if k.client == client:
                    msg = ("" + k.username + "," + k.email + "," + str(k.admin))
                    self.sendMessageToClient(msg, 1, client)
                    break

    def sendMessageToClient(self, msg, action, client):
        self.refresh()
        package = str(len(msg)) + "," + str(action) + ":" + msg
        client.sendall(package.encode('utf-8'))

    def sendMessageToAll(self, msg, action):
        self.refresh()
        package = str(len(msg)) + "," + str(action) + ":" + msg
        for user in self.users:
            user.client.send((package).encode('utf-8'))

if __name__ == "__main__":
    chatServer()
    chatServer().listen()

"""
    def removeUser(self, user):
        user.client.close()
        for users in self.users:
            if "[closed]" in str(users.client):
                print(users.username+" :removeUser")
        self.users.remove(user)
        print("Client disconnected (" + user.username + ")")
"""