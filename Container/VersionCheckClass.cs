using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Container
{
    public class VersionCheckClass : IEquatable<VersionCheckClass>
    {
        int FirstVersionNumber;     // 大版本迭代号，当message存在变动时请及时更新该数字，服务器将会校验该版本；
        int SecondVersionNumber;    // 小版本迭代号
        string UpdateTime;          // 程序上次更新的时间 example: 202100807

        public VersionCheckClass(int firstVersionNumber, int secondVersionNumber, string updateTime)
        {
            FirstVersionNumber = firstVersionNumber;
            SecondVersionNumber = secondVersionNumber;
            if (updateTime == null)
            {
                throw new ArgumentNullException(nameof(updateTime));
            }
            UpdateTime = updateTime;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VersionCheckClass);
        }

        public bool Equals(VersionCheckClass other)
        {
            return other != null &&
                   FirstVersionNumber == other.FirstVersionNumber &&
                   SecondVersionNumber == other.SecondVersionNumber &&
                   UpdateTime == other.UpdateTime;
        }
        
        public override int GetHashCode()
        {
            int hashCode = -427018499;
            hashCode = hashCode * -1521134295 + FirstVersionNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + SecondVersionNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UpdateTime);
            return hashCode;
        }

        public static bool operator ==(VersionCheckClass left, VersionCheckClass right)
        {
            return EqualityComparer<VersionCheckClass>.Default.Equals(left, right);
        }

        public static bool operator !=(VersionCheckClass left, VersionCheckClass right)
        {
            return !(left == right);
        }
        public bool CheckVersion(VersionCheckClass other)
        {
            if (other.FirstVersionNumber == this.FirstVersionNumber)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public static class ServerVersion
    {
        public static VersionCheckClass Version;
        static ServerVersion()
        {
            int firstVersionNumber = 1;
            int secondVersionNumber = 1;
            string updateTime = "20210808";
            Version = new VersionCheckClass(firstVersionNumber,secondVersionNumber,updateTime);
        }
    }
    
    public static class ClientVersion
    {
        public static VersionCheckClass Version;
        static ClientVersion()
        {
            int firstVersionNumber = 1;
            int secondVersionNumber = 1;
            string updateTime = "20210808";
            Version = new VersionCheckClass(firstVersionNumber, secondVersionNumber, updateTime);
        }
    }
}
