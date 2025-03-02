using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityPython;

public class GameManager : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI modelText; [SerializeField] private TextMeshProUGUI inputText; [SerializeField] private TextMeshProUGUI sendText; [SerializeField] private TextMeshProUGUI replyText; [SerializeField] private TextMeshProUGUI tokenText;
	[SerializeField] private LlamaCppConnection llamaCppConnection;
	private string[] modelPath; private int modelIndex; private int modelNum; private string modelName; private string maxTokenText; private int maxToken;

	void OnEnable() {
		modelPath = Directory.GetFiles(Application.streamingAssetsPath + "/GGUF/", "*.gguf", SearchOption.TopDirectoryOnly);
		modelIndex = -1; modelNum = modelPath.Count();
	}
	void Start() {
		ChangeModelText();
	}
	void Update() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			SendText();
		}
	}
	
	public void ChangeModelText() {
		modelIndex = (modelIndex+1)%modelNum;
		modelName = Path.GetFileName(modelPath[modelIndex]);
		modelText.text = Path.GetFileNameWithoutExtension(modelName);
	}
	public async void SendText() {
		string userInput = inputText.text;
		sendText.text = userInput; inputText.text = "";
		replyText.text = "回答生成中";
		maxTokenText = tokenText.text;
		if (maxTokenText.CompareTo("\n") == 1) {
			maxToken = 1024;
		}
		else {
			maxToken = int.Parse(maxTokenText.Substring(0, maxTokenText.Length-1));
		}
		string resultText = await llamaCppConnection.LlamaReply(Application.streamingAssetsPath+"/GGUF/"+modelName, userInput, maxToken);
		replyText.text = resultText;
	}
}
