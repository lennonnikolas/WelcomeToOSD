import { createProxyMiddleware } from 'http-proxy-middleware'
import { fromNodeMiddleware } from 'h3'

export default fromNodeMiddleware(
  createProxyMiddleware({
    target: 'http://localhost:5147',
    changeOrigin: true,
    pathRewrite: { '^/api': '' }
  })
)