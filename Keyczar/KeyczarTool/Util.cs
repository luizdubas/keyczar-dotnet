﻿/*  Copyright 2012 James Tuley (jay+code@tuley.name)
 * 
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyczarTool
{
    internal class Util
    {
        public static string PromptForPassword()
        {
            Console.WriteLine("Please enter passphrase:");
            return Console.ReadLine();
        }

        public static string DoublePromptForPassword()
        {

            int i = 0;
            while (i++ < 4)
            {
                Console.WriteLine("Please enter passphrase:");
                var phrase1 = Console.ReadLine();
                Console.WriteLine("Please re-enter passphrase:");
                var phrase2 = Console.ReadLine();

                if (phrase1.Equals(phrase2))
                {
                    return phrase1;
                }
                Console.WriteLine("Passphrase didn't match.");
            }
            Console.WriteLine("Giving up.");
            throw new Exception("Entered non matching password too many times");
        }
    }
}