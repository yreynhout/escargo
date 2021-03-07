SHELL = /bin/sh

all:
	# Clean distribution
	rm -rf .dist
	# Copy assets
	cp -r -p static .dist
	# Render html documents
	dotnet run -c Release -p shell/Shell.fsproj -- --output=.dist --domain=knowledgecrunchers.io --googlerecaptcha=6Ldic3UaAAAAAE7HDjzDhBInQkOZp4JnWK0ntfSv
	# Generate tailwind stylesheet
	NODE_ENV=production npx tailwindcss-cli build -o .dist/css/tailwind.css
ci:
	# Copy assets
	cp -r -p static .dist
	# Render html documents
	dotnet run -c Release -p shell/Shell.fsproj -- --output=.dist --domain=knowledgecrunchers.io --googlerecaptcha=6Ldic3UaAAAAAE7HDjzDhBInQkOZp4JnWK0ntfSv
	# Generate tailwind stylesheet
	NODE_ENV=production npx tailwindcss-cli build -o .dist/css/tailwind.css
	# Release to AWS S3 bucket
	# aws s3 sync --delete --acl public-read .dist/ s3://knowledgecrunchers.io
serve:
	# Clean distribution
	rm -rf .dist
	# Copy assets
	cp -r -p static .dist
	# Generate tailwind stylesheet
	npx tailwindcss-cli build -o .dist/css/tailwind.css
	# Render html documents
	dotnet run -c Release -p shell/Shell.fsproj -- --output=.dist --domain=knowledgecrunchers.io --googlerecaptcha=6Ldic3UaAAAAAE7HDjzDhBInQkOZp4JnWK0ntfSv
	# Start watching, rendering and serving html documents
	make -j 2 dotnet-serve dotnet-watch
dotnet-serve:
	dotnet serve -o -d .dist
dotnet-watch:
	dotnet watch run -c Release -p shell/Shell.fsproj -- --output=../.dist --domain=knowledgecrunchers.io --googlerecaptcha=6Ldic3UaAAAAAE7HDjzDhBInQkOZp4JnWK0ntfSv
format:
	# Format F# code
	dotnet fantomas shell/