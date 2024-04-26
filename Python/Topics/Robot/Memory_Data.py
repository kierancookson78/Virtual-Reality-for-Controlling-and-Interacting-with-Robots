class Memory_Data():
    #keys
    #joints:
    #head
    #head yaw

    #end head yaw

    #end head
    #end joints
    #end keys

    @staticmethod
    def getActiveDiagnosis(diagnosis_proxy):
        ALvalue = diagnosis_proxy.getActiveDiagnosis()
        print "failure message content: " + str(ALvalue)
        return ALvalue

    @staticmethod
    def getPassiveDiagnosis(diagnosis_proxy):
        ALvalue = diagnosis_proxy.getPassiveDiagnosis()
        print "failure message content:  " + str(ALvalue)
        return ALvalue

    @staticmethod
    def shutDownOnFailure(diagnosis_proxy, speech_service, motion_service):
        ALvalue = Memory_Data.getActiveDiagnosis(diagnosis_proxy)

        if len(ALvalue) > 0:
            information = "Es scheint ein Problem zu geben. Problemstatus: "
            if ALvalue[0] == 1:
                information += "1 Ernst"
            if ALvalue[0] == 2:
                information += "2 Kritisch"

            problemString = ""
            problemList = ALvalue[1]
            for problem in problemList:
                problemString += problem + ", "

            information += "Problempunkt: " + problemString

            speech_service.say(information)

            if ALvalue[0] == 1:
                motion_service.rest()

    @staticmethod
    def goToChargingStation(motion_service, recharge_service):

        motion_service.wakeUp()

        success = recharge_service.goToStation()
        if success != 0:  # The charging station has not been found.
            print "Station is not found."
            return
        print "Robot successfully docked."

    @staticmethod
    def leaveChargingStation(recharge_service):
        recharge_service.leaveStation()

    @staticmethod
    def isCharging():
        print ""

    @staticmethod
    def getChargingStatus(recharge_service):
        return recharge_service.getStatus()

    @staticmethod
    def getBatteryCharge(battery_service):
        return battery_service.getBatteryCharge()

    @staticmethod
    def getBatteryHatchOpen(memory_service):
        return memory_service.getData("Device/SubDeviceList/Platform/ILS/Sensor/Value")

    @staticmethod
    def interpretContent(memory_service, connection, content):
        command = content["content"]

        data = memory_service.getData(command["data_key"])
        connection.sendall(str(data).encode())

