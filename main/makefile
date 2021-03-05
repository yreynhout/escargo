SHELL = /bin/sh

all:
	# Clean distribution
	rm -rf .dist
	# Copy assets
	cp -r -p static .dist
	# Render html documents
	dotnet run -c Release -p escargo/Escargo.fsproj -- --output=.dist --domain=knowledgecrunchers.io
	# Generate tailwind stylesheet
	NODE_ENV=production npx tailwindcss-cli build -o .dist/css/tailwind.css
ci:
	# Copy assets
	cp -r -p static .dist
	# Render html documents
	dotnet run -c Release -p escargo/Escargo.fsproj -- --output=.dist --domain=knowledgecrunchers.io
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
	# Render and serve html documents
	dotnet watch run -c Release -p escargo/Escargo.fsproj -- --output=../.dist --domain=knowledgecrunchers.io \
	& dotnet serve -o -d .dist