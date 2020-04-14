using System;
using System.Collections.Generic;
using System.Text;

namespace Casino.Core.Util {
    public static class Misc {

        public static Random randomNumber = new Random();
        public static int FindFirstNullIndex<T>(T[] arr) {
            for(int pos = 0; pos < arr?.Length; pos++) {
                if (arr[pos] == null) return pos;
            }
            return -1;
        }

    }
}
