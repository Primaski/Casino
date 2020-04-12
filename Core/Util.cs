using System;
using System.Collections.Generic;
using System.Text;

namespace Casino.Core {
    public static class Util {

        public static int FindFirstNullIndex<T>(T[] arr) {
            for(int pos = 0; pos < arr?.Length; pos++) {
                if (arr[pos] == null) return pos;
            }
            return -1;
        }

    }
}
