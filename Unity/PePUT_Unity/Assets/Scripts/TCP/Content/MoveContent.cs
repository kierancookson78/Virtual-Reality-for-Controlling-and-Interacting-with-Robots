using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

[Serializable]
public class MoveContent : Content
{
	public enum MOVE_COMMAND
    {
		move_move,
		move_toward,
		move_moveTo,
		move_isActive,
		move_waitMoveFinished,
		move_stopMove,
		move_getRobotPosition,
		move_getNextRobotPosition,
		move_getRobotVelocity,
		move_getMoveConfig,
    }

	public string move_command;
	public float x;
	public float y;
	public float theta;
	public string move_config;


	public MoveContent(MOVE_COMMAND move_command, float x = 0.0f, float y = 0.0f, float theta = 0.0f, List<MoveConfig> move_config = null)
    {
		this.move_command = move_command.ToString();
		this.x = (float)Math.Round(x, 2);
		this.y = (float)Math.Round(y, 2);
		this.theta = (float)Math.Round(theta, 2);
		this.move_config = JsonUtility.ToJson(move_config);
		Debug.Log(this.move_config);
	}

	public MoveContent(MOVE_COMMAND move_command, string x = "", string y = "", string theta = "", List<MoveConfig> move_config = null)
	{
		this.move_command = move_command.ToString();
		this.x = (float)Math.Round(float.Parse(x, CultureInfo.InvariantCulture.NumberFormat), 2);
		this.y = (float)Math.Round(float.Parse(y, CultureInfo.InvariantCulture.NumberFormat), 2);
		this.theta = (float)Math.Round(float.Parse(theta, CultureInfo.InvariantCulture.NumberFormat), 2);
		this.move_config = JsonUtility.ToJson(move_config);
		Debug.Log(this.move_config);
	}

	public MoveContent(MOVE_COMMAND move_command)
	{
		this.move_command = move_command.ToString();
		this.x = 0f;
		this.y = 0f;
		this.theta = 0f;
		this.move_config = JsonUtility.ToJson(move_config);
		Debug.Log(this.move_config);
	}

	public struct MoveConfig
    {
		public enum ValueType
        {
			//Name 	  															Default 	Minimum 	Maximum 	Settable
			MaxVelXY,		// 	maximum planar velocity (meters/second) 		0.35 		0.1 		0.55 		yes
			MaxVelTheta,    // 	maximum angular velocity (radians/second) 		1.0 		0.2 		2.00 		yes
			MaxAccXY,       // 	maximum planar acceleration (meters/second²) 	0.3 		0.1 		0.55 		yes
			MaxAccTheta,    //  maximum angular acceleration (radians/second²) 	0.75 		0.1 		3.00 		yes
			MaxJerkXY,      //  maximum planar jerk (meters/second³) 			1.0 		0.2 		5.00 		yes
			MaxJerkTheta,	//  maximum angular jerk (radians/second³) 			2.0 		0.2 		50.00 		yes
	}

		public string typeString;
		public float value;

		public MoveConfig(ValueType type, float value)
        {
			typeString = type.ToString();
			this.value = value;
        }
    }
}
