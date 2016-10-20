using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PriemLib
{
    public class DBConstants
    {
        public const string CS_PRIEM = "Data Source=81.89.183.112;Initial Catalog=Priem2016;Integrated Security=SSPI";
        public const string CS_PRIEM_PING = "Data Source=81.89.183.112;Initial Catalog=Priem2016;Integrated Security=false;user=PriemPingUser; password=PriemPingUser_ReadOnly;Connect Timeout=300";
        //PriemSECOND
        public const string CS_PRIEM_SECOND = "Data Source=81.89.183.112;Initial Catalog=PriemSECOND;Integrated Security=SSPI";
        //public const string CS_PRIEM = "Data Source=81.89.183.112;Initial Catalog=Priem;Integrated Security=false; user=Priem2014TestUser; password=Priem2014TestUser; Connect Timeout=10";
        public const string CS_PriemONLINE =
            "Data Source=srvpriem1.ad.pu.ru;Initial Catalog=OnlinePriem2015;Integrated Security=false; user=Priem2012User; password=2012Priem!Okay,kids;Connect Timeout=300";
        public const string CS_PriemONLINE_Files =
            "Data Source=srvpriem1.ad.pu.ru;Initial Catalog=OnlineAbitFiles;Integrated Security=false; user=Priem2012User; password=2012Priem!Okay,kids;Connect Timeout=300";
        public const string CS_PriemONLINE_ReadWrite =
            "Data Source=srvpriem1.ad.pu.ru;Initial Catalog=OnlinePriem2015;Integrated Security=True;Connect Timeout=10";
        public const string CS_PRIEM_FAC = "Data Source=81.89.183.112;Initial Catalog=Priem_TEST;Integrated Security=false; user=Priem2014TestUser; password=Priem2014TestUser; Connect Timeout=10";
        public const string CS_STUDYPLAN = "Data Source=81.89.183.112;Initial Catalog=Education;Integrated Security=true";
    }

    public enum PriemType
    {
        Priem,
        PriemAspirant,
        PriemMag,
        PriemSPO,
        PriemForeigners,
        PriemAG
    }
}