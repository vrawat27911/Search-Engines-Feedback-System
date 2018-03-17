using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchingGoogle
{
    public class UserData
    {
        private int userID;
        private int mAge;
        private string mFirstName;
        private string mLastName;
        private int mGenderCode;

        public int UserID
        {
            get
            {
                return userID;
            }

            set
            {
                userID = value;
            }
        }

        public int MAge
        {
            get
            {
                return mAge;
            }

            set
            {
                mAge = value;
            }
        }

        public string MFirstName
        {
            get
            {
                return mFirstName;
            }

            set
            {
                mFirstName = value;
            }
        }

        public string MLastName
        {
            get
            {
                return mLastName;
            }

            set
            {
                mLastName = value;
            }
        }

        public int MGenderCode
        {
            get
            {
                return mGenderCode;
            }

            set
            {
                mGenderCode = value;
            }
        }

        public UserData(int mAge, string mFirstName, string mLastName, int mGenderCode, int pUserId)
        {
            this.UserID = pUserId;
            this.MAge = mAge;
            this.MFirstName = mFirstName;
            this.MLastName = mLastName;
            if (mGenderCode == 1 || mGenderCode == 2 || mGenderCode == 3)
            {
                this.MGenderCode = mGenderCode;
            }
        }

        public UserData()
        {

        }
    }
}
