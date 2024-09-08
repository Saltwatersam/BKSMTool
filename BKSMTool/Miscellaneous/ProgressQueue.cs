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

using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;

namespace BKSMTool.Miscellaneous
{
    /// <summary>
    /// A thread-safe queue that manages the processing of progress updates asynchronously.
    /// This class is designed to handle progress updates in a sequential manner, ensuring
    /// that each update is processed only after the previous one is completed. It is 
    /// particularly useful in UI applications where updates need to be applied sequentially
    /// to avoid race conditions or UI thread contention.
    /// </summary>
    /// <typeparam name="T">The type of the progress value that will be reported.</typeparam>
    public class ProgressQueue<T>
    {
        // The action to be executed when a progress update is reported, typically a UI update.
        private readonly Action<T> _updateAction;

        // A thread-safe queue to store progress values that need to be processed.
        private readonly ConcurrentQueue<T> _progressQueue = new ConcurrentQueue<T>();

        // Indicates whether the queue is currently being processed.
        private bool _isProcessingQueue;

        // Indicates whether the last progress update has been completed.
        private bool _isProgressUpdateCompleted = true;

        // A lock object to ensure thread-safe access when processing the queue.
        private readonly object _queueLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressQueue{T}"/> class.
        /// </summary>
        /// <param name="updateAction">The action to perform when a progress value is processed.</param>
        public ProgressQueue(Action<T> updateAction)
        {
            _updateAction = updateAction;
        }

        /// <summary>
        /// Reports a new progress value and enqueues it for processing.
        /// </summary>
        /// <param name="value">The progress value to be reported.</param>
        public async void Report(T value)
        {
            // Queue the progress value.
            _progressQueue.Enqueue(value);

            // Start processing the queue asynchronously.
            await ProcessQueue();
        }

        /// <summary>
        /// Processes the queue asynchronously, ensuring that only one update is processed
        /// at a time. If the queue is already being processed, it returns immediately.
        /// </summary>
        private async Task ProcessQueue()
        {
            // Lock the queue to check if it's already being processed.
            lock (_queueLock)
            {
                if (_isProcessingQueue)
                    return;

                _isProcessingQueue = true;
            }

            // Dequeue and process each progress value.
            while (_progressQueue.TryDequeue(out T progressValue))
            {
                // Wait until the previous update is completed.
                while (!_isProgressUpdateCompleted)
                {
                    await Task.Yield(); // Yield control to allow other operations to proceed.
                }

                // Set the flag to indicate a new update is in progress.
                _isProgressUpdateCompleted = false;

                // Update the UI or handle the progress value.
                UpdateProgress(progressValue);
            }

            // After processing all items, release the lock.
            lock (_queueLock)
            {
                _isProcessingQueue = false;
            }
        }

        /// <summary>
        /// Invokes the update action to handle the progress value and marks the update as completed.
        /// </summary>
        /// <param name="value">The progress value to be updated.</param>
        private void UpdateProgress(T value)
        {
            if (_updateAction == null) return;
            // Execute the update action (e.g., updating the UI).
            _updateAction(value);

            // Mark the progress update as completed.
            _isProgressUpdateCompleted = true;
        }

        /// <summary>
        /// Waits until all items in the queue have been processed and the queue is empty.
        /// </summary>
        public async Task WaitForQueueToEmptyAsync()
        {
            // Continuously check if the queue is empty and processing is complete.
            while (_isProcessingQueue || !_progressQueue.IsEmpty)
            {
                await Task.Yield(); // Yield control while waiting.
            }
        }
    }
}
