import time
from naoqi import ALProxy
import almath

from config import ROBOT_PORT

tts = ALProxy("ALTextToSpeech", "127.0.0.1", ROBOT_PORT)
tts.say("Hallo Welt")
posture = ALProxy("ALRobotPosture", "127.0.0.1", ROBOT_PORT)
posture.goToPosture("StandInit", 0.5)


motion = ALProxy("ALMotion", "127.0.0.1", ROBOT_PORT)
'''
motion.setStiffnesses("Head", 1.0)

names = "HeadYaw"
angles = 30.0*almath.TO_RAD
fractionMaxSpeed = 0.3
motion.setAngles(names, angles, fractionMaxSpeed)

time.sleep(3.0)
motion.setStiffnesses("Head", 0.0)

motion.setStiffnesses("RShoulderPitch", 1.0)

names = "RShoulderPitch"
angles = 30.0*almath.TO_RAD
fractionMaxSpeed = 0.3
motion.setAngles(names, angles, fractionMaxSpeed)

time.sleep(3.0)
motion.setStiffnesses("RShoulderPitch", 0.0)
'''
motion.setStiffnesses("RShoulderRoll", 1.0)

names = "RShoulderRoll"
angles = -90.0*almath.TO_RAD
fractionMaxSpeed = 0.3
motion.setAngles(names, angles, fractionMaxSpeed)

time.sleep(3.0)
motion.setStiffnesses("RShoulderRoll", 0.0)



