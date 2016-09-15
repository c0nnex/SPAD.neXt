// PMDG 777 EFIS Demo Script

using SPAD.neXt.Interfaces;
using SPAD.neXt.Interfaces.Aircraft.CDU;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Logging;
using SPAD.neXt.Interfaces.Scripting;
using System;
using System.Threading;

namespace PMDG777_EFIS_Control
{
    public class EfisHelper : IScriptCreation
    {
        public static ILogger logger = null;

        public static ICDUScreen GetCDU(IApplication app, CDU_NUMBER whichCDU)
        {
            ICDUScreen cdu = app.CurrentAircraft?.GetCDU(whichCDU);
            if ((cdu == null) || (!cdu.IsValid))
            {
                logger.Warn("CDU not valid");
                return null;
            }
            if (!cdu.IsPowered)
            {
                logger.Warn("CDU unpowered");
                return null;
            }
            return cdu;
        }

        public static string GetPageName(ICDUScreen cdu)
        {
            var val = cdu?.GetRow(0).Trim();
            logger.Info($"GetPagename = '{val}'");
            return val;
        }

        public static bool WaitForPage(ICDUScreen cdu, string pageName)
        {
            int tries = 0;
            while ((EfisHelper.GetPageName(cdu) != pageName))
            {
                tries++;
                Thread.Sleep(1000);
                if (tries >= 5)
                {
                    logger.Error($"Cannot switch to page {pageName}");
                    return false;
                }
            }
            return true;
        }

        public static bool SwitchAndWaitForPage(ICDUScreen cdu, string pageName, CDU_KEYS keyForSwitching)
        {
            if (EfisHelper.GetPageName(cdu) != pageName)
            {
                cdu.SendKey(keyForSwitching);
                return WaitForPage(cdu, pageName);
            }
            return true;
        }

        public void Deinitialize()
        {

        }

        public void Initialize(IApplication app)
        {
            logger = app.GetLogger("777EFIS");
            logger.Info("EfisHelper initialized");
            // We want to get notified when CDU is available to initialize EFIS Setup
            app.SubscribeToSystemEvent(SPADSystemEvents.CDUAvailable, CDUAvailable);

        }

        public void CDUAvailable(object sender, ISPADEventArgs e)
        {
            logger.Info($"EfisHelper: CDUAvailable {e}");
            logger.Info($"Sender {sender}");
            ICDUScreen cdu = e.NewValue as ICDUScreen;
            logger.Info($"CDU {cdu}");
            if ((cdu != null) && (cdu.IsValid) && (cdu.IsPowered) && (cdu.CDUNumber == CDU_NUMBER.Captain))
            {
                logger.Info("CDU Available");
                if (!SwitchAndWaitForPage(cdu, "MENU", CDU_KEYS.KEY_MENU))
                    return;
                var efisLine = cdu.GetRow(4, 19);
                if (efisLine != "EFIS>")
                {
                    logger.Info($"'{efisLine}' != 'EFIS>'");
                    cdu.SendKey(CDU_KEYS.KEY_R1); // Enable Efis
                }
            }
        }
    }

    public class Minimum_Change : IScriptAction, IScriptCreation
    {
        private ILogger logger = null;

        public void Initialize(IApplication app)
        {
            logger = app.GetLogger("777EFIS");
            logger.Info("777 EFIS Control Initalized");
        }

        public void Deinitialize()
        {

        }

        public void Execute(IApplication app, ISPADEventArgs eventArgs)
        {
            ICDUScreen cdu = EfisHelper.GetCDU(app, CDU_NUMBER.Captain);
            if (cdu == null)
                return;

            if (EfisHelper.GetPageName(cdu) != "EFIS CONTROL")
            {
                if (!EfisHelper.SwitchAndWaitForPage(cdu, "MENU", CDU_KEYS.KEY_MENU))
                    return;
            }
            if (!EfisHelper.SwitchAndWaitForPage(cdu, "EFIS CONTROL", CDU_KEYS.KEY_R2))
                return;

            var oldVal = 0;
           
            for (int i = 0; i < 6; i++)
            {
                var cell = cdu.GetCell(6, i);
                if (!Char.IsDigit(cell.Symbol))
                    break;
                oldVal = oldVal * 10 + (cell.Symbol - '0');
            }
            oldVal += Convert.ToInt32(eventArgs.NewValue);
            logger.Info($"new Value = {oldVal}");
            while (!cdu.GetCell(13, 0).IsEmpty)
            {
                cdu.SendKey(CDU_KEYS.KEY_CLR);
                Thread.Sleep(100);
            }

            var newVal = oldVal.ToString();
            for (int i = 0; i < newVal.Length; i++)
            {
                cdu.SendKey(CDU_KEYS.KEY_0 + (newVal[i] - '0'));
            }
            Thread.Sleep(100);
            cdu.SendKey(CDU_KEYS.KEY_L3);
        }

    }
}
