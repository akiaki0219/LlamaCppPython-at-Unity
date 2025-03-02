from llama_cpp import Llama
def LlamaCpp(model_path, user_input, max_tokens):
  return Llama(model_path=model_path, n_gpu_layers=-1, chat_format="llama-3", n_ctx=8192).create_chat_completion(messages=[{"role": "system","content": "ユーザの質問に対して、140文字程度の返事を返してください。"}, {"role": "user","content": user_input}], max_tokens=max_tokens)["choices"][0]["message"]["content"]