using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Control_Generation_Module
{
    /*
    public enum CGM_Mode { Active, Passive };
    public enum CGM_ResolveType { Speed, InPlane, Higth };

    // Модуль формирования управляющий воздействий
    public interface ICGM
    {
        IDanger DCurrent { get; }
        bool CanIResolveDangers { get; set; }
    }

    public class Control_Generation_Module: ICGM
    {
        public CGM_Mode Mode {
            get;
            protected set;
        } = CGM_Mode.Passive;


        public bool Switch = false;
        public bool CanIResolveDangers { get; set; } = true;

        GPS _gps;
        UAV_Frandly _master;

        List<Danger> DangersList;
        public List<IDanger> DList => DangersList.ConvertAll(new Converter<Danger, IDanger>(DtoID));
        public static IDanger DtoID(Danger d) { return d; }

        public Danger DangerCurrent { get; private set; }
        public IDanger DCurrent { get => DangerCurrent; }

        public Control_Generation_Module(UAV_Frandly master, GPS gps)
        {
            _master = master;
            _gps = gps;
        }

        public void Analise()
        {
            // Обновление угроз
            foreach(var danger in DangersList)
            {
                danger.UpdateCrashPoint();
                danger.UpdateCSpeads();
            }

            //Пролетит ли?
            foreach (var point in DangersList) {
                if(point.LVL == 0)
                    continue;
                if (_master.Speed > point.SpeedCriticalMin || _master.Speed < point.SpeedCriticalMax)
                {
                    point.LVL = 0;
                }
            }

            //Сортировка по удаленности своего
            DangersList.Sort(new ComparatorDangersByDuration());
            for (int k = 0; k < DangersList.Count; ++k) {
                if (DangersList[k].LVL == 0)
                    continue;
                DangersList[k].LVL = (k + 1);
            }

            // Переключаем режим на Активный
            if (Mode == CGM_Mode.Passive && DangersList.Count > 0 && DangersList[0].LVL > 0)
            {
                DangerCurrent = DangersList[0];
                Mode = CGM_Mode.Active;
            }

        }


        public void Resolve ()
        {
            if (Mode != CGM_Mode.Active) return;

            bool danger_changed = DangerCurrent != DangersList[0];

            if (danger_changed && DangersList.Count > 0 && DangersList[0].LVL > 0)
            {
                DangerCurrent = DangersList[0];
            }

            if (DangerCurrent.LVL > 0 && DangerCurrent.Fr_CrashDuration >= 1.5 * DangerCurrent.CriticalLength)
            {
                // Выбор манёвра
                //if (ResolveType == null)
                {
                    try {
                        if(DangerCurrent.SpeedCriticalMin <= UAV_Settings.UAV_Confines.UAV_Params_Confines.MaxValues["Speed"] && _master.Speed > (DangerCurrent.SpeedCriticalMax + DangerCurrent.SpeedCriticalMin) / 2)
                        {
                            double dSpeed = Danger.AccelCalculation(_master.Speed, DangerCurrent.SpeedCriticalMin, DangerCurrent.Fr_CrashDuration, +0.03);
                            _master.Accell = dSpeed;
                        }

                        else if(DangerCurrent.SpeedCriticalMax >= UAV_Settings.UAV_Confines.UAV_Params_Confines.MinValues["Speed"] && _master.Speed <= (DangerCurrent.SpeedCriticalMax + DangerCurrent.SpeedCriticalMin) / 2)
                        {
                            double dSpeed = Danger.AccelCalculation(_master.Speed, DangerCurrent.SpeedCriticalMax, DangerCurrent.Fr_CrashDuration, -0.03);
                            _master.Accell = dSpeed;
                        }

                        else
                        {
                            throw new UAV_Settings.UAV_Exception.UAV_State_Param_Overflow("Speed", DangerCurrent.SpeedCriticalMax);
                        }

                        if (DangerCurrent.ResolveType == null)
                            DangerCurrent.ResolveType = CGM_ResolveType.Speed;
                    } catch (UAV_Settings.UAV_Exception.UAV_State_Param_Overflow) { };

                    // Пробуем другие виды манёвра
                }
            }

            if(!danger_changed)
            {
                // Манёвр завершен - Скоростной
                //if (_master.Speed > CrashPoints[0].SpeedCriticalMax || _master.Speed < CrashPoints[0].SpeedCriticalMin)
                //{
                //    _master.Accell = 0;
                //}

                // Пройдена точка столкновения - Скоростной антиманёвр
                if(DangerCurrent.Resolved && _master.Speed != _master.SpeedStart)
                {
                    double dSpeed = Danger.AccelCalculation(_master.Speed, _master.SpeedStart, UAVBase.GetDuration(_master, _master.TrackSegmentCurrent.End) / 3);
                    try
                    {
                        _master.Accell = dSpeed;
                    } catch (UAV_Settings.UAV_Exception.UAV_State_Param_Overflow)
                    {
                        _master.Accell = UAV_Settings.UAV_Confines.UAV_Params_Confines.MaxValues["Accel"];
                    }
                }

                // Переход в пассивный режим - Скоростной
                if(DangerCurrent.Resolved && Math.Abs(_master.Speed - _master.SpeedStart) < 0.0001 || Switch)
                {
                    _master.Accell = 0;
                    DangerCurrent.ResolveType = null;
                    DangerCurrent = null;
                    Mode = CGM_Mode.Passive;
                }

                Switch = false;
            }
            
            //if(CrashPoints.Count < 1 && CrashPoint)
            //{
            //    _master.Accell = 0;
            //    ResolveType = null;
            //    Mode = CGM_Mode.Passive;
            //}
                
        }

        public CGM_Mode JustDoIT()
        {

            if(DangersList == null)
            {
                DangersList = new List<Danger>();
                for(int i = 0; i < GPS_Data.uavs.Count; i++)
                {
                    if(_master != GPS_Data.uavs[i])
                    DangersList.Add(new Danger(_master, GPS_Data.uavs[i]));
                }
            }

            else
            {
                Analise();

                if(CanIResolveDangers && Mode == CGM_Mode.Active)
                    Resolve();

            }

            for(int i = 0; i < DangersList.Count; ++i)
            { DangersList[i].StashUAV(); }

            return Mode;
        }
    }


    public interface IDanger {
        bool HaveCrashPoint { get; }
        double XCrash { get; }
        double YCrash { get; }

        UAV_Frandly UAV1 { get; }
        UAVBase UAV2 { get; }

        bool HistoryDefined { get; }
        UAVState UAV1_Prev { get; }
        UAVState UAV2_Prev { get; }

        int LVL { get; }
        CGM_ResolveType? ResolveType { get; }
        bool Resolved { get; }

        double Alpha_G { get; }
        double Alpha_R { get; }
        double Beta_G { get; }
        double Beta_R { get; }

        bool CSpeedsDefined { get; }
        double SpeedCriticalMax { get; }
        double SpeedCriticalMin { get; }

        double Fr_CrashDuration { get; }
        double St_CrashDuration { get; }
        double Fr_StDuration { get; }
        double FrontFarthes { get; }
    }

    public class Danger: IDanger
    {
        public bool HaveCrashPoint { get; private set; }
        public double XCrash { get; private set; }
        public double YCrash { get; private set; }

        public UAV_Frandly UAV1 { get; protected set; }
        public UAVBase UAV2 { get; protected set; }

        public bool HistoryDefined { get => (UAV1_Prev != null) && (UAV2_Prev != null); }
        public UAVState UAV1_Prev { get; protected set; }
        public UAVState UAV2_Prev { get; protected set; }

        public int LVL { get; set; } = 0;
        public CGM_ResolveType? ResolveType { get; set; } = null;
        public bool Resolved { get => Fr_StDuration / UAVBase.GetDuration(UAV1_Prev, UAV2_Prev) > 1; }

        // α — Угол слета к точке столкновения
        public double CosAlpha { get {
                double vec_Fr_Cr_x = XCrash - UAV1.X;
                double vec_Fr_Cr_y = YCrash - UAV1.Y;
                double vec_St_Cr_x = XCrash - UAV2.X;
                double vec_St_Cr_y = YCrash - UAV2.Y;
                double Fr_CrashDuration = UAVBase.GetDuration(UAV1.X, UAV1.Y, XCrash, YCrash);
                double St_CrashDuration = UAVBase.GetDuration(UAV2.X, UAV2.Y, XCrash, YCrash);
                double cosAlpha = (vec_Fr_Cr_x * vec_St_Cr_x + vec_Fr_Cr_y * vec_St_Cr_y) / (Fr_CrashDuration * St_CrashDuration);
                return cosAlpha;
        } }
        public double Alpha_R {
            get {
                double alpha_r = Math.Acos(CosAlpha);
                return alpha_r;
            }
        }
        public double Alpha_G {
            get {
                double alpha_g = Alpha_R * 180 / Math.PI;
                return alpha_g;
            }
        }

        // β — Угол между курсом своего и прямой, на которой лежат оба беспилотника
        public double CosBeta {
            get {
                double vec_Fr_Cr_x = Math.Abs(UAV1.XAffter - UAV1.X);
                double vec_Fr_Cr_y = Math.Abs(UAV1.YAffter - UAV1.Y);
                double vec_St_Cr_x = Math.Abs(UAV2.XAffter - UAV1.XAffter);
                double vec_St_Cr_y = Math.Abs(UAV2.YAffter - UAV1.YAffter);
                double ff_dur = UAVBase.GetDuration(UAV1.XAffter, UAV1.YAffter, UAV1.X, UAV1.Y);
                double fs_dur = UAVBase.GetDuration(UAV1.XAffter, UAV1.YAffter, UAV2.XAffter, UAV2.YAffter);
                double cosBeta = (vec_Fr_Cr_x * vec_St_Cr_x + vec_Fr_Cr_y * vec_St_Cr_y) / (ff_dur * fs_dur);
                return cosBeta;
            }
        }
        public double Beta_R {
            get {
                double beta_r = Math.Acos(CosBeta);
                return beta_r;
            }
        }
        public double Beta_G {
            get {
                double beta_g = Beta_R * 180 / Math.PI;
                return beta_g;
            }
        }
        public double Beta { get => Beta_G; }

        public bool CSpeedsDefined { get; private set; }
        public double SpeedCriticalMax { get; protected set; }
        public double SpeedCriticalMin { get; protected set; }

        public double Fr_CrashDuration { get => UAVBase.GetDuration(UAV1.XAffter, UAV1.YAffter, XCrash, YCrash); }
        public double St_CrashDuration { get => UAVBase.GetDuration(UAV2.XAffter, UAV2.YAffter, XCrash, YCrash); }
        public double Fr_StDuration { get => UAVBase.GetDuration(UAV1.XAffter, UAV1.YAffter, UAV2.XAffter, UAV2.YAffter); }
        public double FrontFarthes { get => Danger.FrontFarthestApprox(this.Beta); }

        public double CriticalLength { get => UAV1.Radius + UAV2.Radius; }

        public Danger(UAV_Frandly uav1, UAVBase uav2)
        {
            UAV1 = uav1;
            UAV2 = uav2;
        }

        public void StashUAV()
        {
            UAV1_Prev = UAV1.GetState();
            UAV2_Prev = UAV2.GetState();
        }

        public static double AccelCalculation(double start_speed, double end_speed, double duration, double offset = 0)
        {
            double dSpeed = Math.Round(((2 * end_speed * (end_speed - start_speed) / duration) + offset), 2);
            return dSpeed;
        }

        // Таблица ключевых значений
        public static Dictionary<double, double> F_Table = new Dictionary<double, double>()
        {
            [10] = 1262.3548251166,
            [15] = 1016.41161531033,
            [20] = 866.538305714591,
            [25] = 752.651223255797,
            [30] = 672.947870323861,
            [35] = 609.260177568638,
            [40] = 550.849941517652,
            [45] = 495.766910909014,
            [50] = 447.892479013157,
            [55] = 409.389312213498,
            [60] = 372.24000636279,
            [65] = 335.876179006436,
            [70] = 303.985800328167,
            [75] = 272.543386513336,
            [80] = 245.872006974974,
            [85] = 221.651099129605,
            [90] = 200.818538130697,
            [95] = 182.194038511982,
            [100] = 164.917089641136,
            [105] = 152.284529379816,
            [110] = 141.128857565177,
            [115] = 131.82024589041,
            [120] = 123.208437666555,
            [125] = 117.666522350651,
            [130] = 107.806771398789,
            [135] = 87.3714307592468
        };

        public static double FrontFarthestApprox(double Beta)
        {
            double Rne_02_gr_ap;
            //Rne_02_gr_ap = -1.065 * Math.Pow(10, 7) - 299.2518 * (-3.2956 * Math.Pow(10, 4) - Beta) + 0.0302 * Math.Pow(5.1072 * Math.Pow(10, 3) - Beta, 2) + 1.1873 * Math.Pow(10, -9) * Math.Pow(96.2385 - Beta, 6);

            if (Beta < 10 || Beta > 135)
            {
                throw new ArgumentException();
            }

            if(Beta % 5 == 0)
            {
                Rne_02_gr_ap = F_Table[Beta];
            }

            else
            {
                double b1, b2;
                double r1, r2;

                b1 = Beta - (Beta % 5);
                b2 = b1 + 5;
                r1 = F_Table[b1];
                r2 = F_Table[b2];

                Rne_02_gr_ap = ((Beta - b1) * (r2 - r1) / (b2 - b1)) + r1;
            }

            return Rne_02_gr_ap;
        }

        public void UpdateCrashPoint()
        {
            //Проверка вхождения в мёртвую зону
            if (Beta < 10 || Beta > 130)
            {
                LVL = 0;
                return;
            }
            
            //Проверка НЕвхождения в зону внимания
            if (Fr_StDuration > FrontFarthes)
            {
                LVL = 0;
                return;
            }


            double fr_start_x, fr_start_y;
            double fr_end_x, fr_end_y;
            double st_start_x, st_start_y;
            double st_end_x, st_end_y;
            try
            {
                fr_start_x = UAV1.TrackSegmentCurrent.Start.X;
                fr_start_y = UAV1.TrackSegmentCurrent.Start.Y;
                fr_end_x = UAV1.TrackSegmentCurrent.End.X;
                fr_end_y = UAV1.TrackSegmentCurrent.End.Y;

                st_start_x = UAV2.TrackSegmentCurrent.Start.X;
                st_start_y = UAV2.TrackSegmentCurrent.Start.Y;
                st_end_x = UAV2.TrackSegmentCurrent.End.X;
                st_end_y = UAV2.TrackSegmentCurrent.End.Y;
            } catch (IndexOutOfRangeException)
            {
                XCrash = UAV2.X; YCrash = UAV2.Y;
                HaveCrashPoint = false;
                LVL = 0;
                return;
            }

            // Поиск точки пересечения траекторий 
            double denumerator = (fr_start_x - fr_end_x) * (st_start_y - st_end_y) - (fr_start_y - fr_end_y) * (st_start_x - st_end_x);
            if(denumerator == 0) {
                XCrash = double.NaN; YCrash = double.NaN;
                HaveCrashPoint = false;
                LVL = 0;
            }
            else
            {
                XCrash = ((fr_start_x * fr_end_y - fr_start_y * fr_end_x) * (st_start_x - st_end_x) - (st_start_x * st_end_y - st_start_y * st_end_x) * (fr_start_x - fr_end_x)) / denumerator;
                YCrash = ((fr_start_x * fr_end_y - fr_start_y * fr_end_x) * (st_start_y - st_end_y) - (st_start_x * st_end_y - st_start_y * st_end_x) * (fr_start_y - fr_end_y)) / denumerator;

                // Слет-разлет
                bool flag_FrX = Math.Abs(XCrash - UAV1_Prev.X) > Math.Abs(XCrash - UAV1.XAffter);
                bool flag_FrY = Math.Abs(YCrash - UAV1_Prev.Y) > Math.Abs(YCrash - UAV1.YAffter);
                bool flag_StX = Math.Abs(XCrash - UAV2_Prev.X) > Math.Abs(XCrash - UAV2.XAffter);
                bool flag_StY = Math.Abs(YCrash - UAV2_Prev.Y) > Math.Abs(YCrash - UAV2.YAffter);
                LVL = ((flag_FrX || flag_FrY) && (flag_StX || flag_StY)) ? 1 : 0;

                HaveCrashPoint = true;
            }
        }

        public void UpdateCSpeads()
        {
            if (!HistoryDefined || !HaveCrashPoint)
            {
                CSpeedsDefined = false;
                SpeedCriticalMin = double.NaN;
                SpeedCriticalMax = double.NaN;
                return;
            }

            double Lfot = Fr_CrashDuration / CriticalLength;
            double Lsot = St_CrashDuration / CriticalLength;

            double AV, BV, CV, sD;
            AV = Math.Pow(Lsot, 2) * (1 - Math.Pow(CosAlpha, 2)) - 1;
            BV = 2 * (CosAlpha - Lfot*Lsot*(1 - Math.Pow(CosAlpha, 2)));
            CV = Math.Pow(Lfot, 2) * (1 - Math.Pow(CosAlpha, 2)) - 1;
            sD = Math.Sqrt(BV * BV - 4 * AV * CV);

            double Vf_ot = UAV1.Speed / UAV2.Speed;
            double v1 = (UAV1.Speed * (-BV + sD) / (2 * AV)) / Vf_ot;
            double v2 = (UAV1.Speed * (-BV - sD) / (2 * AV)) / Vf_ot;

            if(v1 > v2)
            {
                SpeedCriticalMin = v1;
                SpeedCriticalMax = v2;
            }
            else
            {
                SpeedCriticalMin = v2;
                SpeedCriticalMax = v1;
            }
            CSpeedsDefined = true;
        }

    }

    internal class ComparatorDangersByDuration : IComparer<Danger> {
        public int Compare(Danger crash_point1, Danger crash_point2) {
            if (crash_point1.Fr_CrashDuration > crash_point2.Fr_CrashDuration)
                return 1;
            if (crash_point1.Fr_CrashDuration < crash_point2.Fr_CrashDuration)
                return -1;
            return 0;
        }
    }
    internal class CopmparatorDangersByDLvl : IComparer<Danger> {
        public int Compare(Danger crash_point1, Danger crash_point2) {
            if (crash_point1.Fr_CrashDuration > crash_point2.Fr_CrashDuration || crash_point1.LVL == 0 && crash_point2.LVL != 0)
                return 1;
            if (crash_point1.Fr_CrashDuration < crash_point2.Fr_CrashDuration || crash_point1.LVL != 0 && crash_point2.LVL == 0)
                return -1;
            return 0;
        }
    }
    */
}
