import { createProxyMiddleware } from 'http-proxy-middleware'

export default createProxyMiddleware({
  target: 'http://localhost:5147',
  changeOrigin: true,
  pathRewrite: {
    '^/api': ''
  }
})