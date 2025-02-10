

#pragma warning disable SKEXP0005
using System;
using System.IO;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.Extensions.DependencyInjection; // 确保添加这个命名空间
using Microsoft.SemanticKernel.Connectors.OpenAI;
namespace _02_05e;
public class TryingOutTheKernel
{
    public static async Task Execute()
    {
        // 获取环境变量和设置部署名称
        var modelDeploymentName = "whisper";
        var azureOpenAIEndpoint = "https://johns-m6bzu4xq-swedencentral.cognitiveservices.azure.com";
        var azureOpenAIApiKey = Environment.GetEnvironmentVariable("AZUREOPENAI_WHISPER_APIKEY");
        //  questions:
        // 1 does it support other languages
        // 2 whats the file limit? is it 25mb as stated?
        // 3 why m4a is not supported in dotnet semantic kernel
        // 4 价格？ 在whisper之前的传统audio to text基本上是50分钟需要几美元（1-5大概？？？），很久之前测过但是忘了
        // 5 速度，决定了是否能够做call center robot, 本地whisperx 或者fast whisper更快
        // 结合RAG ， plugin等等
        // 通过测试，1 支持其他语言，所以微软learning 官网又在搞笑了？？？
        // 2 25mb是限制，超过只能拆分或者用azure ai speech service
        // 3 不清楚？

        // 创建内核构建器并添加音频转文本服务
        var builder = Kernel.CreateBuilder();
        builder.Services.AddAzureOpenAIAudioToText(
            modelDeploymentName,
            azureOpenAIEndpoint,
            azureOpenAIApiKey
        );
        var kernel = builder.Build();

        // 获取音频转文本服务
        var audioToTextService = kernel.Services.GetService<IAudioToTextService>();

        if (audioToTextService == null)
        {
            Console.WriteLine("无法获取音频转文本服务。");
            return;
        }

        // 读取 m4a 文件内容
        var m4aFilePath = "./file.mp3";
        var audioData = File.ReadAllBytes(m4aFilePath);
        // 将 byte[] 转换为 BinaryData
        var binaryAudioData = new BinaryData(audioData);
        AudioContent audioContent = new AudioContent(binaryAudioData );

        OpenAIAudioToTextExecutionSettings settings = new OpenAIAudioToTextExecutionSettings("file.mp3");
        // Configure the Language property
        settings.Language = "de";
        // Configure the Prompt property
       settings.Prompt = "Transkribiere den Inhalt auf Deutsch!";

 
        // 调用音频转文本服务进行转录
        var transcription = await audioToTextService.GetTextContentsAsync(audioContent,null, kernel);


         if (transcription != null && transcription.Count > 0)
        {
            foreach (var textContent in transcription)
            {
                Console.WriteLine(textContent.Text);
            }
        }
        else
        {
            Console.WriteLine("未获取到转录结果。");
        }

      
    }
}