using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace sChakert.Chessboard.Tests
{
    [TestFixture]
    public class StateManagerTests
    {
        // For generating random numbers
        private Random rand = new Random();

        [Test]
        public void SaveCurrentStateTest()
        {
            StateManager.SaveCurrentState();
            Assert.IsTrue((StateManager.GeneralInfoStack.Count > 0) && (StateManager.EnPassantPositionStack.Count > 0));
            StateManager.RestorePreviousState();
        }

        [Test]
        public void RestorePreviousStateTest()
        {
            // Randomize the state
            RandomizeState();
            Debug.WriteLine(StateManager.GetState());
            // Save the current state
            StateManager.SaveCurrentState();
            // Retrieve the current values of the state, used for comparision later on
            var expectedValues = GetSortedStateValues();
            // Restore the state
            StateManager.RestorePreviousState();
            Debug.WriteLine(StateManager.GetState());
            // Verify results
            var fields = typeof(StateManager).GetFields(BindingFlags.Public | BindingFlags.Static);
            /*
            Note that when you push an element to the stack, it will be the first element you retrieve when you call pop next time.
            Because we got the original values using the fields sorted by ascending name,
            we can leave the list of fields sorted by descending name.
            */
            var alphabeticallySortedFields = fields.Where(x => x.FieldType == typeof(int))
                .OrderByDescending(x => x.Name);
            foreach (var field in alphabeticallySortedFields)
                Assert.AreEqual(expectedValues.Pop(), (int) field.GetValue(null));
        }

        /// <summary>
        /// Get the int values of the state alphabatically sorted on name
        /// </summary>
        private Stack<int> GetSortedStateValues()
        {
            var currentValues = new Stack<int>();
            var fields = typeof(StateManager).GetFields(BindingFlags.Public | BindingFlags.Static);
            // Sorted on ascending name
            var alphabeticallySortedFields = fields.Where(x => x.FieldType == typeof(int))
                .OrderByDescending(x => x.Name).Reverse();
            foreach (var field in alphabeticallySortedFields)
            {
                Debug.WriteLine(field.Name + " " + (int) field.GetValue(null));
                currentValues.Push((int) field.GetValue(null));
            }
            return currentValues;
        }

        /// <summary>
        /// Randomize the state 
        /// </summary>
        private void RandomizeState()
        {
            var type = typeof(StateManager);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            // We sort the values on alphabetical order
            var alphabeticallySortedFields = fields.Where(x => x.FieldType == typeof(int))
                .OrderByDescending(x => x.Name).Reverse();
            foreach (var field in alphabeticallySortedFields)
                field.SetValue(field, rand.Next(0, 2));
        }
    }
}