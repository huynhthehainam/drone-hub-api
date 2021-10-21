using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
namespace MiSmart.Infrastructure.Extensions
{
    public class Node
    {
        public String Address { get; set; }
        public Int32 Port { get; set; } = 80;
        public Int32 Weight { get; set; } = 1;
        public String AbsoluteUri
        {
            get
            {
                if (Port == 80)
                {
                    return Address;
                }
                else
                {
                    return $"{Address}:{Port}";
                }
            }
        }
    }
    public class RoundRobinBalancer
    {
        private readonly List<Node> nodes;
        private Int32 i = -1;
        private Int32 cw = 0;
        public RoundRobinBalancer(List<Node> nodes)
        {
            this.nodes = nodes;
        }
        public Node DispatchTo()
        {
            while (true)
            {
                i = (i + 1) % nodes.Count;
                if (i == 0)
                {
                    cw = cw - MaxCommonDivisor(nodes);
                    if (cw <= 0)
                    {
                        cw = MaxWeight(nodes);
                        if (cw == 0)
                            return null;
                    }
                }
                if ((nodes[i]).Weight >= cw)
                    return nodes[i];
            }
        }
        private static Int32 MaxCommonDivisor(List<Node> nodes)
        {
            List<Int32> nums = new List<Int32>();
            foreach (Node node in nodes)
            {
                nums.Add(node.Weight);
            }
            return MaxCommonDivisor(nums);
        }
        private static Int32 MaxWeight(List<Node> nodes)
        {
            Int32 ret = -1;
            foreach (Node node in nodes)
            {
                if (node.Weight > ret)
                    ret = node.Weight;
            }
            return ret;
        }
        public static Int32 GreatestCommonDivisor(Int32 n, Int32 m)
        {
            if (n < m)
            {
                n = m + n;
                m = n - m;
                n = n - m;
            }
            if (m == 0) return n;
            return GreatestCommonDivisor(m, n % m);
        }
        public static Int32 MaxCommonDivisor(List<Int32> several)
        {
            Int32 a = several[0];
            Int32 b = several[1];
            Int32 c = GreatestCommonDivisor(a, b);
            Int32 i;
            for (i = 2; i < several.Count; i++)
            {
                c = GreatestCommonDivisor(c, several[i]);
            }
            return c;
        }
    }
    public static class HttpClientExtensions
    {
        public static HttpResponseMessage TrySendRequest(this HttpClient client, HttpMethod httpMethod, HttpContent content, AuthenticationHeaderValue authenticationHeader, Node node, String localPath)
        {
            return client.TrySendRequestAsync(httpMethod, content, authenticationHeader, node, localPath).Result;
        }
        public static HttpResponseMessage TrySendRequest(this HttpClient client, HttpMethod httpMethod, HttpContent content, AuthenticationHeaderValue authenticationHeader, List<Node> nodes, String localPath)
        {
            return client.TrySendRequestAsync(httpMethod, content, authenticationHeader, nodes, localPath).Result;
        }
        public static Task<HttpResponseMessage> TrySendRequestAsync(this HttpClient client, HttpMethod httpMethod, HttpContent content, AuthenticationHeaderValue authenticationHeader, Node node, String localPath)
        {
            try
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource(4000);
                HttpRequestMessage request = new HttpRequestMessage(httpMethod, $"{node.AbsoluteUri}{localPath}");
                request.Content = content;
                request.Headers.Authorization = authenticationHeader;
                return client.SendAsync(request, cancellationToken: tokenSource.Token);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static Task<HttpResponseMessage> TrySendRequestAsync(this HttpClient client, HttpMethod httpMethod, HttpContent content, AuthenticationHeaderValue authenticationHeader, List<Node> nodes, String localPath)
        {
            Node node = null;
            if (nodes.Count == 0)
            {
                throw new Exception("List nodes must have value");
            }
            else if (nodes.Count == 1)
            {
                node = nodes[0];
            }
            else if (nodes.Count > 1)
            {
                RoundRobinBalancer roundRobinBalancer = new RoundRobinBalancer(nodes);
                node = roundRobinBalancer.DispatchTo();
            }
            return client.TrySendRequestAsync(httpMethod, content, authenticationHeader, node, localPath);
        }
    }
}