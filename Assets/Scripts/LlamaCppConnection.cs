using Python.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPython {
	public class LlamaCppConnection : MonoBehaviour {
    private SemaphoreSlim _semaphoreSlim;

		void OnEnable() {
			_semaphoreSlim = new SemaphoreSlim(1, 1);
		}

		public async Task<string> LlamaReply(string modelName, string userInput, int maxToken) {
			IntPtr? state = null;
			try {
				await _semaphoreSlim.WaitAsync(destroyCancellationToken);
				state = PythonEngine.BeginAllowThreads();
				return await Task.Run(() => LlamaPython(modelName, userInput, maxToken));
			}
			catch (OperationCanceledException e) when (e.CancellationToken != destroyCancellationToken) {
				return "回答の生成に失敗しました";
				throw;
			}
			finally {
				if (state.HasValue) {
					PythonEngine.EndAllowThreads(state.Value);
				}
				_semaphoreSlim.Release();
			}
		}
		private string LlamaPython(string modelName, string userInput, int maxToken) {
			using (Py.GIL()) {
				using dynamic llamaCppPython = Py.Import("LlamaCppPython");
				using dynamic result = llamaCppPython.LlamaCpp(modelName, userInput, maxToken);
				return result;
			}
		}
	}
}
