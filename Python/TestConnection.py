import time
from naoqi import ALProxy
import almath
tts = ALProxy("ALTextToSpeech", "169.254.228.115", 9559)
tts.say("Hallo Welt")
posture = ALProxy("ALRobotPosture", "169.254.228.115", 9559)
posture.goToPosture("StandInit", 0.5)


motion = ALProxy("ALMotion", "169.254.228.115", 9559)
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



