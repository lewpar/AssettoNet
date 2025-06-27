using System;
using System.Runtime.InteropServices;

namespace AssettoNet.Network.Struct
{
    /// <summary>
    /// Contains telemetry about the vehicle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct AssettoUpdateData
    {
        private char identifier;
        private char identifierTerminator;

        private int size;

        private float speedKmh;
        private float speedMph;
        private float speedMs;

        private byte isAbsEnabled;
        private byte isAbsInAction;
        private byte isTcInAction;
        private byte isTcEnabled;
        private byte isInPit;
        private byte isEngineLimiterOn;

        // Re-align struct to 4 byte boundary
        private byte padding1;
        private byte padding2;

        private float accGVertical;
        private float accGHorizontal;
        private float accGFrontal;

        private int lapTime;
        private int lastLap;
        private int bestLap;
        private int lapCount;

        private float gas;
        private float brake;
        private float clutch;

        private float engineRPM;
        private float steer;
        private int gear;

        private float cgHeight;

        private Vec4 wheelAngularSpeed;
        private Vec4 slipAngle;
        private Vec4 slipAngleContactPatch;
        private Vec4 slipRatio;
        private Vec4 tyreSlip;
        private Vec4 ndSlip;
        private Vec4 load;
        private Vec4 dy;
        private Vec4 mz;
        private Vec4 tyreDirtyLevel;
        private Vec4 camberRad;
        private Vec4 tyreRadius;
        private Vec4 tyreLoadedRadius;
        private Vec4 suspensionHeight;

        private float carPositionNormalized;

        private float carSlope;

        private Vec3 carCoordinates;

        /// <summary>
        /// Gets the vehicle speed in kilometers per hour.
        /// </summary>
        public float SpeedKmh { get => speedKmh; }

        /// <summary>
        /// Gets the vehicle speed in miles per hour.
        /// </summary>
        public float SpeedMph { get => speedMph; }

        /// <summary>
        /// Gets the vehicle speed in meters per second.
        /// </summary>
        public float SpeedMs { get => speedMs; }
        
        /// <summary>
        /// Indicates whether ABS (Anti-lock Braking System) is currently enabled.
        /// </summary>
        public bool IsAbsEnabled
        {
            get => isAbsEnabled != 0;
            set => isAbsEnabled = (byte)(value ? 1 : 0);
        }

        /// <summary>
        /// Indicates whether ABS is actively controlling the brakes at this moment.
        /// </summary>
        public bool IsAbsInAction
        {
            get => isAbsInAction != 0;
            set => isAbsInAction = (byte)(value ? 1 : 0);
        }

        /// <summary>
        /// Indicates whether TC (Traction Control) is actively limiting power or applying braking.
        /// </summary>
        public bool IsTcInAction
        {
            get => isTcInAction != 0;
            set => isTcInAction = (byte)(value ? 1 : 0);
        }

        /// <summary>
        /// Indicates whether Traction Control is currently enabled.
        /// </summary>
        public bool IsTcEnabled
        {
            get => isTcEnabled != 0;
            set => isTcEnabled = (byte)(value ? 1 : 0);
        }

        /// <summary>
        /// Indicates whether the car is currently in the pit lane.
        /// </summary>
        public bool IsInPit
        {
            get => isInPit != 0;
            set => isInPit = (byte)(value ? 1 : 0);
        }

        /// <summary>
        /// Indicates whether the engine limiter (e.g., pit speed limiter) is currently active.
        /// </summary>
        public bool IsEngineLimiterOn
        {
            get => isEngineLimiterOn != 0;
            set => isEngineLimiterOn = (byte)(value ? 1 : 0);
        }

        /// <summary>
        /// Gets how far the gas peddle is pressed down in percentage (0-100).
        /// </summary>
        public float GasPeddle { get => gas * 100; }

        /// <summary>
        /// Gets how far the gas peddle is pressed down in percentage (0-100).
        /// </summary>
        public float BrakePeddle { get => brake * 100; }

        /// <summary>
        /// Gets how far the clutch peddle is pressed down in percentage (0-100).
        /// </summary>
        public float ClutchPeddle { get => clutch * 100; }

        /// <summary>
        /// Gets the current RPM of the engine.
        /// </summary>
        public float EngineRPM { get => engineRPM; }

        /// <summary>
        /// Gets the current gear of the vehicle.
        /// <para>0 = Reverse, 1 = Neutral, 2 = First Gear, 3 = Second Gear, etc.</para>
        /// </summary>
        public int Gear { get => gear; }
        
        /// <summary>
        /// Gets the centre of gravity height of the vehicle relative to the ground.
        /// </summary>
        public float CentreOfGravityHeight { get => cgHeight; }

        /// <summary>
        /// Gets the vehicles position on the track as a percentage (0-100).
        /// </summary>
        public float Position { get => carPositionNormalized * 100; }

        /// <summary>
        /// Gets the vechicle x,y,z coordinates on the map.
        /// </summary>
        public Vec3 Coordinates { get => carCoordinates; }

        /// <summary>
        /// Gets the angular speed of each wheel in radians per second.
        /// </summary>
        public Vec4 WheelAngularSpeed { get => wheelAngularSpeed; }

        /// <summary>
        /// Gets the slip angle of each tire, representing the difference between the direction the tire is pointing and the actual direction of travel.
        /// </summary>
        public Vec4 SlipAngle { get => slipAngle; }

        /// <summary>
        /// Gets the slip angle of each tire measured at the contact patch where the tire meets the road.
        /// </summary>
        public Vec4 SlipAngleContactPatch { get => slipAngleContactPatch; }

        /// <summary>
        /// Gets the slip ratio of each tire, indicating the difference between wheel speed and vehicle speed.
        /// </summary>
        public Vec4 SlipRatio { get => slipRatio; }

        /// <summary>
        /// Gets the amount of slip occurring in each tire.
        /// </summary>
        public Vec4 TyreSlip { get => tyreSlip; }

        /// <summary>
        /// Gets the normalized slip value for each tire.
        /// </summary>
        public Vec4 NdSlip { get => ndSlip; }

        /// <summary>
        /// Gets the vertical force applied to each wheel.
        /// </summary>
        public Vec4 Load { get => load; }

        /// <summary>
        /// Gets a telemetry value related to lateral force or displacement.
        /// </summary>
        public Vec4 Dy { get => dy; }

        /// <summary>
        /// Gets the self-aligning torque applied to each tire.
        /// </summary>
        public Vec4 Mz { get => mz; }

        /// <summary>
        /// Gets the dirt or debris level affecting the grip of each tire.
        /// </summary>
        public Vec4 TyreDirtyLevel { get => tyreDirtyLevel; }

        /// <summary>
        /// Gets the camber angle of each wheel in radians.
        /// </summary>
        public Vec4 CamberRad { get => camberRad; }

        /// <summary>
        /// Gets the unloaded radius of each tire, measured when no load is applied.
        /// </summary>
        public Vec4 TyreRadius { get => tyreRadius; }

        /// <summary>
        /// Gets the effective radius of each tire when under load.
        /// </summary>
        public Vec4 TyreLoadedRadius { get => tyreLoadedRadius; }

        /// <summary>
        /// Gets the suspension height at each wheel.
        /// </summary>
        public Vec4 SuspensionHeight { get => suspensionHeight; }

        /// <summary>
        /// Gets the slope of the car, representing the angle of incline or decline.
        /// </summary>
        public float CarSlope { get => carSlope; }
        
        /// <summary>
        /// Gets the current lap time.
        /// </summary>
        public int LapTime => lapTime;

        /// <summary>
        /// Gets the last completed lap time.
        /// </summary>
        public int LastLap => lastLap;

        /// <summary>
        /// Gets the best lap time achieved so far.
        /// </summary>
        public int BestLap => bestLap;

        /// <summary>
        /// Gets the number of laps completed.
        /// </summary>
        public int LapCount => lapCount;
        
        /// <summary>
        /// Gets the vertical G-force (acceleration) acting on the vehicle.
        /// </summary>
        public float AccelerationGVertical => accGVertical;
        
        /// <summary>
        /// Gets the horizontal G-force acting on the vehicle during cornering.
        /// </summary>
        public float AccelerationGHorizontal => accGHorizontal;
        
        /// <summary>
        /// Gets the frontal G-force acting on the vehicle during acceleration or braking.
        /// </summary>
        public float AccelerationGFrontal => accGFrontal;

        /// <returns>
        /// A formatted string containing the all of the vehicle telemetry.
        /// </returns>
        public override string ToString()
        {
            return $"Speed(Kmh): {SpeedKmh:F0}, Speed(Mph): {SpeedMph:F0}, Speed(Ms): {SpeedMs:F0}" + Environment.NewLine +
                   $"G-Forces: Vertical: {AccelerationGVertical:0.00}g, Horizontal: {AccelerationGHorizontal:0.00}g, Frontal: {AccelerationGFrontal:0.00}g" + Environment.NewLine +
                   $"GasPeddle: {GasPeddle:00.00}%, BrakePeddle: {BrakePeddle:00.00}%, ClutchPeddle: {ClutchPeddle:00.00}%" + Environment.NewLine +
                   $"EngineRPM: {EngineRPM}, Gear: {Gear}" + Environment.NewLine +
                   $"CentreOfGravityHeight: {CentreOfGravityHeight}, Coordinates: {Coordinates}, Position: {Position:00.00}%" + Environment.NewLine +
                   $"WheelAngularSpeed: {WheelAngularSpeed}, SlipAngle: {SlipAngle}, SlipAngleContactPatch: {SlipAngleContactPatch}" + Environment.NewLine +
                   $"SlipRatio: {SlipRatio}, TyreSlip: {TyreSlip}, NdSlip: {NdSlip}" + Environment.NewLine +
                   $"Load: {Load}, Dy: {Dy}, Mz: {Mz}" + Environment.NewLine +
                   $"TyreDirtyLevel: {TyreDirtyLevel}, CamberRad: {CamberRad}" + Environment.NewLine +
                   $"TyreRadius: {TyreRadius}, TyreLoadedRadius: {TyreLoadedRadius}, SuspensionHeight: {SuspensionHeight}" + Environment.NewLine +
                   $"CarSlope: {CarSlope}" + Environment.NewLine +
                   $"IsAbsEnabled: {IsAbsEnabled}, IsAbsInAction: {IsAbsInAction}, IsTcEnabled: {IsTcEnabled}, IsTcInAction: {IsTcInAction}" + Environment.NewLine +
                   $"IsInPit: {IsInPit}, IsEngineLimiterOn: {IsEngineLimiterOn}" + Environment.NewLine +
                   $"LapTime: {LapTime} ms, LastLap: {LastLap} ms, BestLap: {BestLap} ms, LapCount: {LapCount}";
        }
    }
}
