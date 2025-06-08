using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace TcpServer
{
    public class RoomServerLauncher
    {
        public readonly Kubernetes _client;

        public RoomServerLauncher()
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            _client = new Kubernetes(config);
        }

        public async Task<string> LaunchRoomServerAsync(string roomId)
        {
            var podName = $"room-server-{roomId}";

            var pod = new V1Pod
            {
                Metadata = new V1ObjectMeta
                {
                    Name = podName,
                    Labels = new Dictionary<string, string>
                    {
                        ["app"] = podName
                    }
                },
                Spec = new V1PodSpec
                {
                    Containers = new List<V1Container>
                    {
                        new V1Container
                        {
                            Name = "room-server",
                            Image = "game-server-room:latest",
                            ImagePullPolicy = "Never", // 로컬 이미지 사용
                            Command = new List<string> { "dotnet", "RoomServer.dll", "7778" },
                            Ports = new List<V1ContainerPort>
                            {
                                new V1ContainerPort(7778)
                            },
                            EnvFrom = new List<V1EnvFromSource>
                            {
                                new V1EnvFromSource
                                {
                                    SecretRef = new V1SecretEnvSource
                                    {
                                        Name = "room-server-secret"
                                    }
                                }
                            }
                        }
                    },
                    RestartPolicy = "Never"
                }
            };

            await _client.CreateNamespacedPodAsync(pod, "default");
            return podName;
        }

        public async Task WaitForRoomReadyAsync(string podName, int timeoutSeconds = 30)
        {
            var start = DateTime.UtcNow;
            while ((DateTime.UtcNow - start).TotalSeconds < timeoutSeconds)
            {
                var pod = await _client.ReadNamespacedPodAsync(podName, "default");
                if (pod.Status.Phase == "Running" && pod.Status.PodIP != null)
                {
                    return;
                }
                await Task.Delay(1000);
            }
            throw new TimeoutException($"RoomServer {podName} not ready after {timeoutSeconds} seconds.");
        }

        public async Task<string> GetPodIPAsync(string podName)
        {
            var pod = await _client.ReadNamespacedPodAsync(podName, "default");
            return pod.Status.PodIP;
        }

        public async Task DeleteRoomAsync(string podName)
        {
            await _client.DeleteNamespacedPodAsync(podName, "default", new V1DeleteOptions());
        }
    }
}