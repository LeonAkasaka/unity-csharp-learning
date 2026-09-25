@echo off
echo Jekyll プレビューを起動しています...
echo ブラウザで http://localhost:4000/unity-csharp-learning/ を開いてください。
echo 停止するには Ctrl+C を押してください。
echo.

docker run --rm -it ^
  -v "%~dp0docs:/srv/jekyll" ^
  -w /srv/jekyll ^
  -p 4000:4000 ^
  ruby:3.3 ^
  sh -c "bundle install && bundle exec jekyll serve --host 0.0.0.0 --watch --force_polling"
