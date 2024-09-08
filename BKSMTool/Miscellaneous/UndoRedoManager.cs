// Copyright (c) 2024 Saltwatersam
// 
// This file is part of BKSMTool.
// 
// BKSMTool is licensed under the GPLv3 License:
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using BKSMTool.Files.File_WEM;

namespace BKSMTool.Miscellaneous
{
    /// <summary>
    /// Manages the execution, undoing, and redoing of commands in an application.
    /// This class maintains a stack of executed commands for undo operations and a separate
    /// stack for redo operations, enabling reversible actions. It is useful in scenarios
    /// where users need to revert or reapply changes, such as in text editors or graphic applications.
    /// </summary>
    public class UndoRedoManager
    {
        // Stack to keep track of commands that can be undone.
        private readonly Stack<WemCommand> _undoStack = new Stack<WemCommand>();

        // Stack to keep track of commands that can be redone after an undo operation.
        private readonly Stack<WemCommand> _redoStack = new Stack<WemCommand>();

        /// <summary>
        /// Executes a command and adds it to the undo stack.
        /// Clears the redo stack since redoing is not possible after a new command is executed.
        /// </summary>
        /// <param name="command">The command to be executed.</param>
        public void ExecuteCommand(WemCommand command)
        {
            command.Execute();
            _undoStack.Push(command);

            // Clear the redo stack when a new command is executed, as redo operations
            // are no longer valid in this context.
            _redoStack.Clear();
        }

        /// <summary>
        /// Undoes the last executed command, moving it to the redo stack.
        /// </summary>
        public void Undo()
        {
            // Ensure there is a command to undo.
            if (_undoStack.Count <= 0) return;

            // Pop the last command from the undo stack and unexecute it.
            var command = _undoStack.Pop();
            command.Unexecute();

            // Push the command onto the redo stack so it can be redone later.
            _redoStack.Push(command);
        }

        /// <summary>
        /// Redoes the last undone command, moving it back to the undo stack.
        /// </summary>
        public void Redo()
        {
            // Ensure there is a command to redo.
            if (_redoStack.Count <= 0) return;

            // Pop the last command from the redo stack and re-execute it.
            var command = _redoStack.Pop();
            command.Execute();

            // Push the command back onto the undo stack.
            _undoStack.Push(command);
        }

        /// <summary>
        /// Clears both the undo and redo stacks, resetting the manager.
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        /// <summary>
        /// Determines whether there are commands available to undo.
        /// </summary>
        public bool CanUndo => _undoStack.Count > 0;

        /// <summary>
        /// Determines whether there are commands available to redo.
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;
    }
}
