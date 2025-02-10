

#pragma warning disable SKEXP0005
using System;
using System.IO;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.Extensions.DependencyInjection; // 确保添加这个命名空间
namespace _02_05e;
public class TryingOutTheKernel
{
    public static async Task Execute()
    {
        // 获取环境变量和设置部署名称
        var modelDeploymentName = "whisper";
        var azureOpenAIEndpoint = "https://johns-m6bzu4xq-swedencentral.cognitiveservices.azure.com";
        var azureOpenAIApiKey = Environment.GetEnvironmentVariable("AZUREOPENAI_WHISPER_APIKEY");
      

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
        var m4aFilePath = "./rendition.mp3";
        var audioData = File.ReadAllBytes(m4aFilePath);
        // 将 byte[] 转换为 BinaryData
        var binaryAudioData = new BinaryData(audioData);
        AudioContent audioContent = new AudioContent(binaryAudioData );

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